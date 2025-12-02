using System;
using GameContext.CurrencySystem;
using GameContext.VisualTransactions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace GameContext
{
    public class MockGameplayView : MonoBehaviour
    {
        [Header("Simulate")]
        [SerializeField]
        private Button _simulateGameplayButton;
        [SerializeField]
        private TextMeshProUGUI _simulateGameplayButtonText;

        [Header("Datetime Offset")]
        [SerializeField]
        private TMP_InputField _dateTimeOffsetDayInput;
        [SerializeField]
        private TMP_InputField _dateTimeOffsetHourInput;
        [SerializeField]
        private TMP_InputField _dateTimeOffsetMinuteInput;
        [SerializeField]
        private TMP_InputField _dateTimeOffsetSecondInput;
        [SerializeField]
        private Button _applyDateTimeOffsetButton;
        
        [Header("Earn Currency")]
        [SerializeField]
        private TMP_InputField _earnCurrencyAmountInput;
        [SerializeField]
        private TMP_Dropdown _earnCurrencyDropdown;
        [SerializeField]
        private Button _earnCurrencyButton;
        [SerializeField]
        private Button _disposeAnimationButton;
        
        [Inject]
        private MockGameplayModel _model;
        [Inject]
        private CurrencyDefinitions _currencyDefinitions;
        [Inject]
        private IVisualTransactionManager _visualTransactionManager;
        private string _defaultSimulateButtonText;

        private ReactiveProperty<IDisposable> _visualTransactionDisposable;

        private void Start()
        {
            SetupSimulateGameplayBehaviour();
            SetupDatetimeOffsetBehaviour();
            SetupEarnCurrencyBehaviour();
        }

        #region Currency Behaviour

        private void SetupEarnCurrencyBehaviour()
        {
            _visualTransactionDisposable = new ReactiveProperty<IDisposable>(null).AddTo(this);
            _visualTransactionDisposable.Subscribe(disposable =>
            {
                _disposeAnimationButton.interactable = disposable != null;
            }).AddTo(this);
            _earnCurrencyButton.OnClickAsObservable()
                .Subscribe(OnAddCurrencyClicked)
                .AddTo(this);
            _disposeAnimationButton.OnClickAsObservable()
                .Subscribe(OnDisposeAnimation)
                .AddTo(this);
        }

        private void OnAddCurrencyClicked()
        {
            var currencyId = GetSelectedCurrencyId();
            var amount = GetCurrencyAmountFromInput();
            _model.AddCurrency(currencyId, GetCurrencyAmountFromInput());
            if (amount > 0)
                _visualTransactionDisposable.Value = _visualTransactionManager.PerformVisualOnlyTransaction(new Transaction()
                    {
                        id = currencyId,
                        amount = amount
                    },
                    _earnCurrencyButton.GetComponent<RectTransform>(), CurrencyViewContext.None);
        }

        private int GetCurrencyAmountFromInput()
        {
            return _earnCurrencyAmountInput.text == "" ? 0 : int.Parse(_earnCurrencyAmountInput.text);
        }

        private string GetSelectedCurrencyId()
        {
            return _currencyDefinitions.CurrencyDefinitionList.Find(def =>
                def.VisibleName == _earnCurrencyDropdown.options[_earnCurrencyDropdown.value].text).UniqueIdentifier;
        }

        private void OnDisposeAnimation()
        {
            _visualTransactionDisposable.Value?.Dispose();
            _visualTransactionDisposable.Value = null;
        }

        #endregion

        #region Datetime Offset Behaviour

        private void SetupDatetimeOffsetBehaviour()
        {
            _applyDateTimeOffsetButton.OnClickAsObservable()
                .Subscribe(OnApplyDatetimeOffsetButtonClicked)
                .AddTo(this);
        }

        private void OnApplyDatetimeOffsetButtonClicked()
        {
            if (TryParseDatetimeOffset(out TimeSpan offset))
                _model.SetDatetimeOffset(offset);
        }
        
        private bool TryParseDatetimeOffset(out TimeSpan offset)
        {
            offset = TimeSpan.Zero;
            if (int.TryParse(_dateTimeOffsetDayInput.text, out var days) &&
                int.TryParse(_dateTimeOffsetHourInput.text, out var hours) &&
                int.TryParse(_dateTimeOffsetMinuteInput.text, out var minutes) &&
                int.TryParse(_dateTimeOffsetSecondInput.text, out var seconds))
            {
                offset = new TimeSpan(days, hours, minutes, seconds);
                return true;
            }

            return false;
        }

        #endregion

        #region Simulate Gameplay Behaviour

        private void SetupSimulateGameplayBehaviour()
        {
            _defaultSimulateButtonText = _simulateGameplayButtonText.text;
            _model.CanSimulateGameplay
                .Subscribe(OnCanSimulateGameplayChanged)
                .AddTo(this);
            _model.SimulateGameplayCooldown
                .Subscribe(OnSimulateGameplayCooldownChanged)
                .AddTo(this);
            _simulateGameplayButton.OnClickAsObservable()
                .Subscribe(OnSimulateGameplay)
                .AddTo(this);
        }
        private void OnSimulateGameplay()
        {
            _model.SimulateGameplay(out var transaction);
            if (transaction.amount > 0)
                _visualTransactionManager.PerformVisualOnlyTransaction(transaction,
                    _simulateGameplayButton.GetComponent<RectTransform>(), CurrencyViewContext.None);
        }

        private void OnSimulateGameplayCooldownChanged(int cooldown)
        {
            _simulateGameplayButtonText.text = cooldown > 0 ? cooldown.ToString() : _defaultSimulateButtonText;
        }

        private void OnCanSimulateGameplayChanged(bool canSimulate)
        {
            _simulateGameplayButton.interactable = canSimulate;
        }

        #endregion
    }
}