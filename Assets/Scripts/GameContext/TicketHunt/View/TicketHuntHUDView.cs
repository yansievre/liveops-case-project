using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameContext.CurrencySystem;
using GameContext.CurrencySystem.Views;
using GameContext.PopupManagement;
using GameContext.VisualTransactions;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace GameContext.TicketHunt
{
    public class TicketHuntHUDView : MonoBehaviour, ICurrencyView
    {
        public class Factory : PlaceholderFactory<TicketHuntHUDView>
        {
        }
        [Header("References")]
        [SerializeField, Required]
        private GameObject _eventHudParent;
        [SerializeField, Required]
        private Image _ticketImage;
        [SerializeField, Required]
        private Image _nextRewardImage;
        [SerializeField, Required]
        private TextMeshProUGUI _timeRemainingText;
        [SerializeField, Required]
        private TextMeshProUGUI _ticketsText;
        [SerializeField, Required]
        private RectTransform _progressBarRect;
        [SerializeField, Required]
        private Button _openEventButton;
        [Header("Assets")]
        [SerializeField, Required]
        private Sprite _multiRewardSprite;
        [Header("Settings")]
        [SerializeField]
        private Vector2 _progressBarAnchors = new Vector2(0, 1);
        [SerializeField]
        private float _progressBarLerpSpeed = 5;
        [SerializeField]
        private UnityEvent _onHitWithParticle;

        private DisposableBag _enabledLifetime;
        private RectTransform _rectTransform;

        #region Injection
        private TicketHuntModel _model;
        private ICurrencyManager _currencyManager;
        private IPopupManager _popupManager;
        private ICurrencyViewDatabase _currencyViewDatabase;
        private IVisualTransactionManager _visualTransactionManager;
        
        
        [Inject]
        public void Construct(TicketHuntModel model, ICurrencyManager currencyManager, IPopupManager popupManager, ICurrencyViewDatabase currencyViewDatabase, IVisualTransactionManager visualTransactionManager)
        {
            _model = model;
            _currencyManager = currencyManager;
            _popupManager = popupManager;
            _currencyViewDatabase = currencyViewDatabase;
            _visualTransactionManager = visualTransactionManager;
        }
        #endregion

        private List<CurrencyViewOffset> _activeOffsets = new List<CurrencyViewOffset>();
        private long _currentTotalOffset;
        private List<VisualTransactionHandle> _queuedTransactions = new List<VisualTransactionHandle>();
        
        private void Start()
        {
            _currentTotalOffset = 0;
            _openEventButton.OnClickAsObservable()
                .Subscribe(_ => _popupManager.ShowTicketHuntPopup())
                .AddTo(this);
            _rectTransform = GetComponent<RectTransform>();
            var anchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.DOAnchorPos(anchoredPos, 1f).From(new Vector2(anchoredPos.x, anchoredPos.y + _rectTransform.rect.height)).SetEase(Ease.InSine);
        }
        
        private void OnEnable()
        {
            _enabledLifetime = new DisposableBag();
            _currencyViewDatabase.RegisterView(this);
            
            _model.TimeRemaining.Subscribe(timeRemaining =>
            {
                if (timeRemaining <= TimeSpan.Zero)
                {
                    _timeRemainingText.text = "00:00:00";
                    return;
                }

                _timeRemainingText.text = timeRemaining.FormatTimeSpan();
            }).AddTo(ref _enabledLifetime);
            _model.OnRewardGiven.Subscribe(x =>
            {
                foreach (var transaction in x.transactions)
                {
                    var handle = _visualTransactionManager.CreateVisualOnlyTransaction(transaction);
                    if(handle == null)
                        continue;
                    handle.ApplyVisualOffset();
                    _queuedTransactions.Add(handle);
                }
            }).AddTo(ref _enabledLifetime);

            _currentVisualLevel = _model.CurrentLevel.CurrentValue;
            _currentTickets = _model.EventCurrencyHandle.Amount.CurrentValue;
            _currentMaxTickets = _model.CurrentStep.CurrentValue.RequiredCurrency;
            _lastVisibleTickets = _currentTickets;
            _nextRewardImage.sprite = GetRewardSpriteForLevel(_currentVisualLevel);

            PlayQueuedTransactions();
        }
        private void OnDisable()
        {
            _currencyViewDatabase.UnregisterView(this);
            _enabledLifetime.Dispose();
        }

        private int _currentVisualLevel;
        private long _currentTickets;
        private int _currentMaxTickets;
        private bool _readyForDestroy = false;

        private long _lastVisibleTickets;
        
        private void LateUpdate()
        {
            if(_readyForDestroy)
            {
                return;
            }
            //Real tickets increased
            if (_model.EventCurrencyHandle.Amount.CurrentValue > _currentTickets)
            {
                _currentTickets = _model.EventCurrencyHandle.Amount.CurrentValue;
            }
            var currentTicketsWithOffset = _currentTickets + _currentTotalOffset;
            currentTicketsWithOffset = Math.Min(currentTicketsWithOffset, _currentMaxTickets);
            if (currentTicketsWithOffset < _lastVisibleTickets)
            {
                currentTicketsWithOffset = _lastVisibleTickets;
            }
            _lastVisibleTickets = currentTicketsWithOffset;
            UpdateProgressBarText(currentTicketsWithOffset, _currentMaxTickets);
            UpdateProgressBarFill((float) currentTicketsWithOffset/_currentMaxTickets);

            if (currentTicketsWithOffset >= _currentMaxTickets)
            {
                _currentTickets = 0;
                _lastVisibleTickets = 0;
                _currentVisualLevel = _model.CurrentLevel.CurrentValue;
                if(_currentVisualLevel < _model.Steps.Count)
                    _currentMaxTickets = _model.Steps[_currentVisualLevel].RequiredCurrency;
                else
                    _currentMaxTickets = 1;
                _nextRewardImage.sprite = GetRewardSpriteForLevel(_currentVisualLevel);
                PlayQueuedTransactions();

                if (_model.IsCompleted.CurrentValue)
                {
                    _readyForDestroy = true;
                    var anchoredPos = _rectTransform.anchoredPosition;
                    _rectTransform.DOAnchorPos(new Vector2(anchoredPos.x, anchoredPos.y + _rectTransform.rect.height), 1f)
                        .SetEase(Ease.InSine)
                        .OnComplete(() => Destroy(gameObject));
                }
            }
        }
        
        private void PlayQueuedTransactions()
        {
            foreach (var handle in _queuedTransactions)
            {
                handle.Play(_nextRewardImage.rectTransform);
            }
            _queuedTransactions.Clear();
        }

        private Sprite GetRewardSpriteForLevel(int level)
        {
            if(level < 0 || level >= _model.Steps.Count)
                return null;
            var rewards = _model.Steps[level].RewardPackage;
            if(rewards.transactions.Length > 1)
                return _multiRewardSprite;
            
            var reward = rewards.transactions[0];
            return _currencyManager.GetCurrencyHandle(reward.id).Definition.Icon;
        }


        private void UpdateProgressBarText(long amount, long max)
        {
            _ticketsText.text = $"{amount}/{max}";
        }
        
        private void UpdateProgressBarFill(float progress)
        {
            _progressBarRect.anchorMax = new Vector2(Mathf.Lerp(_progressBarAnchors.x, _progressBarAnchors.y, progress), 1);
        }

        public string CurrencyId => _model.EventCurrencyHandle.Definition.UniqueIdentifier;
        public RectTransform ParticleTarget => _ticketImage.rectTransform;
        public CurrencyViewOffset CreateVisualOffset(long offsetAmount)
        {
            var offset = new CurrencyViewOffset(offsetAmount, OnOffsetChanged);
            _activeOffsets.Add(offset);
            OnOffsetChanged();
            return offset;
        }

        public void OnHitWithParticle()
        {
            _onHitWithParticle?.Invoke();
        }

        private void OnOffsetChanged()
        {
            _activeOffsets.RemoveAll(x => x.IsDisposed);
            _currentTotalOffset = _activeOffsets.Sum(x => x.CurrentOffset);
        }
    }
}