using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameContext.CurrencySystem;
using GameContext.CurrencySystem.Views;
using GameContext.PopupManagement;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace GameContext.TicketHunt.View
{
    public class TicketHuntPopupView : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<TicketHuntPopupView> { }
        [Header("References")]
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private TextMeshProUGUI _timeRemainingText;
        [SerializeField]
        private RectTransform _verticalFillRect;
        [SerializeField]
        private TicketHuntPopupRewardView _grandPrizeView;
        [SerializeField]
        private Transform _rewardViewParent;
        [Header("Asset References")]
        [SerializeField]
        private TicketHuntPopupRewardView _rewardViewPrefab;
        [SerializeField]
        private BasicCurrencyView _singleRewardPrefab;
        
        [Inject]
        private TicketHuntModel _model;
        [Inject]
        private IPopupManager _popupManager;
        [Inject]
        private ICurrencyManager _currencyManager;

        private float _heightPerStep = 150;
        private DisposableBag _disposableBag;
        private ObjectPool<TicketHuntPopupRewardView> _rewardViewPool;
        private ObjectPool<BasicCurrencyView> _singleRewardPool;
        private List<TicketHuntPopupRewardView> _activeRewardViews = new List<TicketHuntPopupRewardView>();
        private List<BasicCurrencyView> _activeSingleRewardViews = new List<BasicCurrencyView>();

        #region Initial setup

        private void Awake()
        {
            SetupPools();
            var stepRectTransform = _rewardViewPrefab.GetComponent<RectTransform>();
            _heightPerStep = stepRectTransform.rect.height;
        }

        private void SetupPools()
        {
            _rewardViewPool = new ObjectPool<TicketHuntPopupRewardView>(
                () => Instantiate(_rewardViewPrefab, _rewardViewParent), 
                view => view.gameObject.SetActive(true), 
                view => view.gameObject.SetActive(false), 
                view => Destroy(view.gameObject), true, 3);

            _singleRewardPool = new ObjectPool<BasicCurrencyView>(
                () => Instantiate(_singleRewardPrefab), 
                view => view.gameObject.SetActive(true), 
                view => view.gameObject.SetActive(false), 
                view => Destroy(view.gameObject), true, 3);
        }

        #endregion

        #region On Popup Enabled

        public void OnEnable()
        {
            //Clean up first just in case
            ReleaseActivePooledViews();
            _disposableBag.Dispose();
            _disposableBag = new DisposableBag();
            
            SetupTimeRemainingField();
            SetupPopupCloseBehaviour();

            var currentLevel = _model.CurrentLevel.CurrentValue;
            SetupRewardAreas(currentLevel);
            Canvas.ForceUpdateCanvases();
            // Delay one frame to ensure layout is updated before calculating fill, otherwise the fill may be incorrect
            UniTask.DelayFrame(1).ContinueWith(() => UpdateVerticalFill(currentLevel, _heightPerStep)).Forget();
        }

        private void SetupRewardAreas(int currentLevel)
        {
            for (int i = 0; i < _model.Steps.Count - 1; i++)
            {
                var step = _model.Steps[i];
                var stepView = _rewardViewPool.Get();

                UpdateStepView(i, currentLevel, stepView, step);
                stepView.transform.SetAsLastSibling();
                _activeRewardViews.Add(stepView);
            }

            UpdateStepView(_model.Steps.Count - 1, currentLevel, _grandPrizeView, _model.Steps[^1]);
        }

        private void SetupTimeRemainingField()
        {
            _model.TimeRemaining.Subscribe(timeRemaining =>
            {
                if(timeRemaining.TotalSeconds < 0)
                    timeRemaining = TimeSpan.Zero;
                _timeRemainingText.text = timeRemaining.FormatTimeSpan();
            }).AddTo(ref _disposableBag);
        }

        private void SetupPopupCloseBehaviour()
        {
            _model.EventLive.Where(isLive => isLive == false)
                .Subscribe(_popupManager.HideTicketHuntPopup)
                .AddTo(ref _disposableBag);
            _closeButton.OnClickAsObservable()
                .Subscribe(_popupManager.HideTicketHuntPopup)
                .AddTo(ref _disposableBag);
        }

        private void UpdateVerticalFill(int currentLevel, float heightPerStep)
        {
            var maxHeight = _verticalFillRect.parent.GetComponent<RectTransform>().rect.height;
            var amountPerStep = heightPerStep / maxHeight;
            var incompleteSteps = _model.Steps.Count - currentLevel - 1;
            var anchorMaxY = 1 - (incompleteSteps * amountPerStep);
            _verticalFillRect.anchorMax = new Vector2(_verticalFillRect.anchorMax.x, Mathf.Clamp01(anchorMaxY));
        }
        
        private void UpdateStepView(int index, int currentLevel, TicketHuntPopupRewardView stepView, TicketHuntStep step)
        {
            if(index < currentLevel)
                stepView.SetAsCompleted();
            else if(index == currentLevel)
                stepView.SetAsInProgress((int)_model.EventCurrencyHandle.Amount.CurrentValue, step.RequiredCurrency, index);
            else
                stepView.SetAsLocked(index);

            foreach (var transaction in step.RewardPackage.transactions)
            {
                var handle = _currencyManager.GetCurrencyHandle(transaction.id);
                _singleRewardPool.Get(out var singleRewardView);
                
                singleRewardView.transform.SetParent(stepView.RewardParent, false);
                singleRewardView.SetIcon(handle.Definition.Icon);
                singleRewardView.SetAmount(handle.GetFormattedAmount(transaction.amount));
                _activeSingleRewardViews.Add(singleRewardView);
            }
        }

        #endregion

        #region On Popup Disabled / Destroyed

        public void OnDisable()
        {
            // Cleanup if necessary
            _disposableBag.Dispose();
        }

        private void OnDestroy()
        {
            _rewardViewPool.Dispose();

            ReleaseActivePooledViews();
        }

        private void ReleaseActivePooledViews()
        {
            foreach (var view in _activeRewardViews)
            {
                if (view != null)
                    _rewardViewPool.Release(view);
            }
            
            foreach (var view in _activeSingleRewardViews)
            {
                if (view != null)
                    _singleRewardPool.Release(view);
            }

            _activeRewardViews.Clear();
            _activeSingleRewardViews.Clear();
        }

        #endregion
    }
}