using System;
using System.Collections.Generic;
using GameContext.CurrencySystem;
using GameContext.CurrencySystem.Views;
using GameContext.UIParticleManagement;
using R3;
using UnityEngine;
using Zenject;

namespace GameContext.VisualTransactions
{
    public class VisualTransactionHandle : IDisposable
    {
        private readonly Transaction _transaction;
        private readonly CurrencyDefinitions _currencyDefinitions;
        private readonly ICurrencyView _currencyView;
        private readonly IUIParticleManager _uiParticleManager;
        private readonly VisualTransactionSettings _settings;
        private DisposableBag _disposableBag = new DisposableBag();
        private TargetedUIParticlePlayHandle _currentParticleHandle;
        private CurrencyViewOffset _visualOffset;

        public VisualTransactionHandle(Transaction transaction, CurrencyDefinitions currencyDefinitions, ICurrencyView currencyView, IUIParticleManager uiParticleManager, VisualTransactionSettings settings)
        {
            _transaction = transaction;
            _currencyDefinitions = currencyDefinitions;
            _currencyView = currencyView;
            _uiParticleManager = uiParticleManager;
            _settings = settings;
        }

        public bool IsComplete => _currentParticleHandle?.IsComplete??false;

        public void ApplyVisualOffset()
        {
            _visualOffset = _currencyView
                .CreateVisualOffset(-_transaction.amount)
                .AddTo(ref _disposableBag);
        }
        
        public void Play(RectTransform source, CurrencyViewContext context = CurrencyViewContext.None)
        {
            var transferPerBurst = CalculateBurstInfo(_transaction.amount, out var bursts);
            _currentParticleHandle = _uiParticleManager.PlayTargetedParticles(new TargetedUIParticlePlayInfo()
            {
                from =  source,
                to = _currencyView.ParticleTarget,
                bursts = bursts,
                burstInterval = _settings.BurstInterval,
                deliverAllParticlesOnDispose = true,
                particleMaterial = _currencyDefinitions.CurrencyDefinitionList.Find(def => def.UniqueIdentifier == _transaction.id)?.UiParticleMaterial
            }).AddTo(ref _disposableBag);
            
            if(_visualOffset == null)
                ApplyVisualOffset();

            _currentParticleHandle.OnDeliveredParticles
                .Subscribe(deliveredAmount =>
                {
                    _visualOffset.Add(deliveredAmount * transferPerBurst);
                    _currencyView.OnHitWithParticle();
                })
                .AddTo(ref _disposableBag);
        }
        
        /// <summary>
        /// amount is the total amount of currency to be transferred. This function calculates how to break that amount into bursts of particles.
        /// It returns how much money should be transferred per particle.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="bursts"></param>
        /// <returns></returns>
        private long CalculateBurstInfo(long amount, out int[] bursts)
        {
            var burstCount = Mathf.Min(_settings.MaxBurstCount,(int) (amount-1) / _settings.MaxParticlesPerBurst) + 1;
            bursts = new int[burstCount];
            if (amount >= _settings.MaxParticlesPerBurst * _settings.MaxBurstCount)
            {
                for (int i = 0; i < burstCount; i++)
                {
                    bursts[i] = _settings.MaxParticlesPerBurst;
                }
                return amount / (burstCount * _settings.MaxParticlesPerBurst);
            }
            else
            {
                long remainingAmount = amount;
                for (int i = 0; i < burstCount; i++)
                {
                    var amountPerBurst = (int)Mathf.Ceil((float)remainingAmount / (burstCount - i));
                    bursts[i] = amountPerBurst;
                    remainingAmount -= amountPerBurst;
                }
                return 1;
            }
            return 1; // This seems to be unused, returning 1.
        }

        public void Dispose()
        {
            _disposableBag.Dispose();
        }
    }
    
    public class VisualTransactionManager : IVisualTransactionManager, ITickable
    {
        private readonly CurrencyDefinitions _currencyDefinitions;
        private readonly ICurrencyViewDatabase _currencyViewDatabase;
        private readonly IUIParticleManager _uiParticleManager;
        private readonly VisualTransactionSettings _settings;
        
        private List<VisualTransactionHandle> _activeHandles = new List<VisualTransactionHandle>();

        public VisualTransactionManager(
            CurrencyDefinitions currencyDefinitions,
            ICurrencyViewDatabase currencyViewDatabase,
            IUIParticleManager uiParticleManager,
            VisualTransactionSettings settings)
        {
            _currencyDefinitions = currencyDefinitions;
            _currencyViewDatabase = currencyViewDatabase;
            _uiParticleManager = uiParticleManager;
            _settings = settings;
        }
        public VisualTransactionHandle CreateVisualOnlyTransaction(Transaction transaction, CurrencyViewContext targetContext = CurrencyViewContext.None)
        {
            var currencyView = _currencyViewDatabase.GetViewForCurrency(transaction.id, targetContext);

            if (currencyView == null)
                return null;
            var handle = new VisualTransactionHandle(transaction, _currencyDefinitions, currencyView, _uiParticleManager, _settings);
            
            _activeHandles.Add(handle);

            return handle;
        }
        public VisualTransactionHandle PerformVisualOnlyTransaction(Transaction transaction, RectTransform source, CurrencyViewContext targetContext = CurrencyViewContext.None)
        {
            var handle = CreateVisualOnlyTransaction(transaction, targetContext);
            if (handle == null)
                return null;
            handle.Play(source, targetContext);
            return handle;
        }

        public void Tick()
        {
            for (int i = _activeHandles.Count - 1; i >= 0; i--)
            {
                if (_activeHandles[i].IsComplete)
                {
                    _activeHandles[i].Dispose();
                    _activeHandles.RemoveAt(i);
                }
            }
        }
    }
}