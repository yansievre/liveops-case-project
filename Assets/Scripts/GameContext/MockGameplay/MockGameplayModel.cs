using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameContext.CurrencySystem;
using GameContext.VisualTransactions;
using ProjectContext.DateTimeSource;
using R3;
using DateTimeOffset = ProjectContext.DateTimeSource.DateTimeOffset;

namespace GameContext
{
    public class MockGameplayModel : IDisposable
    {
        // Injected fields
        private readonly ICurrencyManager _currencyManager;
        private readonly MockGameplaySettings _settings;
        private readonly DateTimeOffset _dateTimeOffset;

        // Clean up
        private DisposableBag _disposableBag = new();
        private CancellationTokenSource _cts;
        
        // Modifiable reactive properties
        private ReactiveProperty<int> _simulateGameplayCooldown;
        
        // Publicly accessible properties
        public ReadOnlyReactiveProperty<int> SimulateGameplayCooldown => _simulateGameplayCooldown;
        public ReadOnlyReactiveProperty<bool> CanSimulateGameplay { get; }
        
        public MockGameplayModel(ICurrencyManager currencyManager, MockGameplaySettings settings, DateTimeOffset dateTimeOffset)
        {
            _currencyManager = currencyManager;
            _settings = settings;
            _dateTimeOffset = dateTimeOffset;
            _simulateGameplayCooldown = new ReactiveProperty<int>(0).AddTo(ref _disposableBag);
            CanSimulateGameplay = _simulateGameplayCooldown.Select(cooldown => cooldown <= 0).ToReadOnlyReactiveProperty().AddTo(ref _disposableBag);
            _cts = new CancellationTokenSource();
        }

        public void SimulateGameplay(out Transaction performedTransaction)
        {
            performedTransaction = default;
            if (!CanSimulateGameplay.CurrentValue)
                return;
            var costHandle = _currencyManager.GetCurrencyHandle(_settings.SimulateGameplayCost.id);
            var rewardHandle = _currencyManager.GetCurrencyHandle(_settings.SimulateGameplayRewardCurrency);
            if(costHandle.Amount.Value >= _settings.SimulateGameplayCost.amount)
            {
                costHandle.Remove(_settings.SimulateGameplayCost.amount);
                _simulateGameplayCooldown.Value = _settings.SimulateGameplayCooldownSeconds;
                var reward = UnityEngine.Random.Range(_settings.SimulateGameplayRewardRange.x, _settings.SimulateGameplayRewardRange.y + 1);
                rewardHandle.Add(reward);
                performedTransaction = new Transaction()
                {
                    id = _settings.SimulateGameplayRewardCurrency,
                    amount = reward
                };
                SimulateGameplayCountdown(_cts.Token).Forget();
            }
        }

        public void SetDatetimeOffset(TimeSpan offset)
        {
            _dateTimeOffset.Offset = offset;
        }
        
        public void AddCurrency(string currencyId, int amount)
        {
            var handle = _currencyManager.GetCurrencyHandle(currencyId);

            if (amount > 0)
                handle.Add(amount);
            else if (amount < 0)
                handle.Remove(-amount);
        }

        private async UniTask SimulateGameplayCountdown(CancellationToken token)
        {
            bool isCancelled = false;
            while(_simulateGameplayCooldown.Value > 0 && !isCancelled)
            {
                isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token).SuppressCancellationThrow();
                _simulateGameplayCooldown.Value--;
            }
        }
        

        public void Dispose()
        {
            _disposableBag.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}