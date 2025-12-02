using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameContext.CurrencySystem;
using GameContext.TicketHunt.DataLoader;
using GlobalContext.Persistence;
using ProjectContext;
using R3;
using Zenject;

namespace GameContext.TicketHunt
{
    public class TicketHuntModel : IDisposable, IInitializable
    {
        private DisposableBag _disposableBag;
        private readonly ICurrencyManager _currencyManager;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly ILiveOpsLoader _liveOpsLoader;
        private readonly ITimeManager _timeManager;

        private readonly TicketHuntData _activeTicketHuntData;
        private LiveOpsStates LiveOpsStates => _gameStateHandler.CurrentState.LiveOpsStates;

        #region Modifiable Reactive Properties
        private readonly ReactiveProperty<int> _currentLevel;
        private readonly Subject<TransactionPackage> _onRewardGiven;
        #endregion

        
        public ReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
        public ReadOnlyReactiveProperty<bool> EventLive { get; }
        public ReadOnlyReactiveProperty<TransactionPackage> NextReward { get; }
        public ReadOnlyReactiveProperty<TimeSpan> TimeRemaining { get; }
        public ReadOnlyReactiveProperty<TicketHuntStep> CurrentStep { get; }
        public ReadOnlyReactiveProperty<bool> IsCompleted { get; }
        public CurrencyHandle EventCurrencyHandle { get; }
        public IReadOnlyList<TicketHuntStep> Steps => _activeTicketHuntData.Steps;
        public Observable<TransactionPackage> OnRewardGiven => _onRewardGiven;
        
        
        public TicketHuntModel(
            ICurrencyManager currencyManager,
            IGameStateHandler gameStateHandler,
            ILiveOpsLoader liveOpsLoader,
            ITimeManager timeManager)
        {
            _currencyManager = currencyManager;
            _gameStateHandler = gameStateHandler;
            _liveOpsLoader = liveOpsLoader;
            _timeManager = timeManager;

            _activeTicketHuntData = liveOpsLoader.TicketHuntData.CurrentValue;
            _currentLevel = new ReactiveProperty<int>(0).AddTo(ref _disposableBag);
            _onRewardGiven = new Subject<TransactionPackage>().AddTo(ref _disposableBag);
            
            var ticketHuntState = GetOrCreateState();
            
            SetupStateProperties(ticketHuntState);

            EventCurrencyHandle = _currencyManager.GetCurrencyHandle(_activeTicketHuntData.CurrencyId);
            EventLive = _timeManager.CurrentUtcTime.Select(IsEventLive)
                .ToReadOnlyReactiveProperty()
                .AddTo(ref _disposableBag);
            NextReward = _currentLevel.CombineLatest(_liveOpsLoader.TicketHuntData, SelectNextReward)
                .ToReadOnlyReactiveProperty()
                .AddTo(ref _disposableBag);
            TimeRemaining = _liveOpsLoader.TicketHuntData.CombineLatest(_timeManager.CurrentUtcTime, (data, time) =>
                    data == null ? TimeSpan.Zero : data.UTCEndTime - time)
                .ToReadOnlyReactiveProperty()
                .AddTo(ref _disposableBag);
            CurrentStep = _currentLevel.Select(x =>
            {
                if (x < 0 || x >= _activeTicketHuntData.Steps.Count)
                    return null;
                return _activeTicketHuntData.Steps[x];
            }).ToReadOnlyReactiveProperty();
            IsCompleted = _currentLevel.Select(x => x >= _activeTicketHuntData.Steps.Count).ToReadOnlyReactiveProperty().AddTo(ref _disposableBag);
        }

        public void Initialize()
        {
            _liveOpsLoader.LoadLiveOpsData().Forget();
            EventCurrencyHandle.Amount.DelayFrame(1).Subscribe(amount =>
            {
                while (CurrentStep.CurrentValue != null && EventCurrencyHandle.CanAfford(CurrentStep.CurrentValue.RequiredCurrency))
                {
                    EventCurrencyHandle.Remove(CurrentStep.CurrentValue.RequiredCurrency);

                    foreach (var transaction in CurrentStep.CurrentValue.RewardPackage.transactions)
                    {
                        _currencyManager.GetCurrencyHandle(transaction.id).Add(transaction.amount);
                    }
                    _onRewardGiven.OnNext(CurrentStep.CurrentValue.RewardPackage);
                    _currentLevel.Value++;
                }
            }).AddTo(ref _disposableBag);
        }
        
        private TicketHuntState GetOrCreateState()
        {
            if(_activeTicketHuntData == null)
                return null;
            if(!LiveOpsStates.TicketHuntData.TryGetValue(_activeTicketHuntData.UniqueEventId, out var ticketHuntState))
            {
                ticketHuntState = new TicketHuntState
                {
                    currentLevel = 0
                };
                LiveOpsStates.TicketHuntData[_activeTicketHuntData.UniqueEventId] = ticketHuntState;
                _gameStateHandler.SaveGameState();
            }

            return ticketHuntState;
        }

        private void SetupStateProperties(TicketHuntState ticketHuntState)
        {
            _currentLevel.Value = ticketHuntState.currentLevel;
            _currentLevel.Subscribe(x =>
            {
                ticketHuntState.currentLevel = x;
                _gameStateHandler.SaveGameState();
            }).AddTo(ref _disposableBag);
        }
        
        private bool IsEventLive(DateTime currentTime)
        {
            if (_activeTicketHuntData == null)
                return false;
            return currentTime >= _activeTicketHuntData.UTCStartTime && currentTime <= _activeTicketHuntData.UTCEndTime;
        }
        
        private TransactionPackage SelectNextReward(int currentLevel, TicketHuntData ticketHuntData)
        {
            if (ticketHuntData == null)
                return null;
            if (currentLevel < 0 || currentLevel >= ticketHuntData.Steps.Count)
                return null;
            return ticketHuntData.Steps[currentLevel].RewardPackage;
        }
        
        public void Dispose()
        {
            _disposableBag.Dispose();
        }

    }
}