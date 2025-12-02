using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utils;
using Zenject;

namespace GameContext.CurrencySystem.Views
{
    public partial class CurrencyView : MonoBehaviour, ICurrencyView
    {
        private ICurrencyManager _currencyManager;
        private ICurrencyViewDatabase _currencyViewDatabase;
        private List<CurrencyViewOffset> _activeOffsets = new List<CurrencyViewOffset>();
        private CurrencyHandle _currencyHandle;
        
        [SerializeField]
        private string _currencyId;
        [SerializeField]
        private BasicCurrencyView _basicCurrencyView;
        [SerializeField]
        private bool _overrideIcon;
        [FormerlySerializedAs("_onCurrencyAdded")]
        [SerializeField]
        private UnityEvent _onHitWithParticle;
        
        
        public string CurrencyId => _currencyId;
        public RectTransform ParticleTarget => _basicCurrencyView.iconImage.rectTransform;
        
        [Inject]
        public void Construct(ICurrencyManager currencyManager, ICurrencyViewDatabase currencyViewDatabase)
        {
            _currencyManager = currencyManager;
            _currencyViewDatabase = currencyViewDatabase;
        }

        private void Start()
        {
            _currencyHandle = _currencyManager.GetCurrencyHandle(_currencyId);
            if(_basicCurrencyView.HasAmount)
                _currencyHandle.Amount.Subscribe(CurrencyAmountChanged).AddTo(this);
            if(_overrideIcon)
                _basicCurrencyView.SetIcon(_currencyHandle.Definition.Icon);
            
        }

        private void CurrencyAmountChanged()
        {
            var realAmount = _currencyHandle.Amount.Value;
            //remove any disposed offsets
            _activeOffsets.RemoveAll(x => x == null || x.IsDisposed);
            var offsetAmount = _activeOffsets.Sum(x => x.CurrentOffset);
            var targetAmount = realAmount + offsetAmount;
            _basicCurrencyView.SetAmount(_currencyHandle.GetFormattedAmount(targetAmount));
        }

        /// <summary>
        /// Will create a offset that will be visually added to the current amount.
        /// </summary>
        /// <param name="offsetAmount">Positive or negative offset</param>
        public CurrencyViewOffset CreateVisualOffset(long offsetAmount)
        {
            var offset = new CurrencyViewOffset(offsetAmount, CurrencyAmountChanged);
            _activeOffsets.Add(offset);
            CurrencyAmountChanged();
            return offset;
        }

        public void OnHitWithParticle()
        {
            _onHitWithParticle?.Invoke();
        }

        private void OnEnable()
        {
            _currencyViewDatabase.RegisterView(this);
        }

        private void OnDisable()
        {
            _currencyViewDatabase.UnregisterView(this);
        }
    }
}