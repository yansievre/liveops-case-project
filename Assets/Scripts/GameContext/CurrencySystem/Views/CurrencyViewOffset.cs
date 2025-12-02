using System;

namespace GameContext.CurrencySystem.Views
{
    public class CurrencyViewOffset : IDisposable
    {
        private readonly Action _updateCallback;
        private long _currentOffset;
        private bool _isDisposed;

        public long CurrentOffset => _currentOffset;
        public bool IsDisposed => _isDisposed;

        public CurrencyViewOffset(long offsetAmount, Action updateCallback)
        {
            _currentOffset = offsetAmount;
            _updateCallback = updateCallback;
        }
        
        public void Add(long amount)
        {
            _currentOffset += amount;
            _updateCallback?.Invoke();
        }

        public void Dispose()
        {
            _currentOffset = 0;
            _isDisposed = true;
            _updateCallback?.Invoke();
        }
    }
    
}