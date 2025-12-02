using System;
using System.Collections.Generic;
using GlobalContext.Persistence;
using R3;

namespace GameContext.CurrencySystem
{
    public class CurrencyHandle : IDisposable
    {
        private readonly IGameStateHandler _gameStateHandler;
        private DisposableBag _disposables;
        public ReactiveProperty<long> Amount { get; }
        
        public CurrencyDefinition Definition { get; }
        public CurrencyHandle(CurrencyDefinition currencyDefinition, IGameStateHandler gameStateHandler)
        {
            _gameStateHandler = gameStateHandler;
            _disposables = new DisposableBag();
            Definition = currencyDefinition;
            Amount = new ReactiveProperty<long>(_gameStateHandler.CurrentState.CurrencyData[currencyDefinition.UniqueIdentifier]);
            Amount.Subscribe(newAmount =>
            {
                _gameStateHandler.CurrentState.CurrencyData[currencyDefinition.UniqueIdentifier] = newAmount;
            }).AddTo(ref _disposables);
        }

        /// <summary>
        /// Adds the specified amount to the currency. Amount must be non-negative.
        /// This updates both the real and visible amounts immediately.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="saveImmediately"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(long amount, bool saveImmediately = true)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative", nameof(amount));
            Amount.Value += amount;
            if (saveImmediately)
                _gameStateHandler.SaveGameState();
        }

        /// <summary>
        /// Removes the specified amount from the currency. Amount must be non-negative and less than or equal to the current real amount.
        /// This updates both the real and visible amounts immediately.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="saveImmediately"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(long amount, bool saveImmediately = true)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative", nameof(amount));
            if (amount > Amount.Value)
                throw new ArgumentException("Amount to remove cannot be greater than the current amount", nameof(amount));
            Amount.Value -= amount;
            if (saveImmediately)
                _gameStateHandler.SaveGameState();
        }

        public void PerformTransaction(Transaction transaction, bool saveImmediately = true)
        {
            if (transaction.id != Definition.UniqueIdentifier)
                throw new ArgumentException($"Transaction currency ID {transaction.id} does not match this handle's currency ID {Definition.UniqueIdentifier}", nameof(transaction));
            if (transaction.amount > 0)
                Add(transaction.amount, saveImmediately);
            else if (transaction.amount < 0)
                Remove(-transaction.amount, saveImmediately);
        }
        
        public bool CanAfford(long amount)
        {
            return amount >= 0 && Amount.Value >= amount;
        }
        
        public string GetFormattedAmount()
        {
            return GetFormattedAmount(Amount.Value);
        }
        public string GetFormattedAmount(long amount)
        {
            return Definition.Prefix + amount.ToString("N0") + Definition.Suffix;
        }
        public void Dispose()
        {
            Amount?.Dispose();
            _disposables.Dispose();
        }
    }
}