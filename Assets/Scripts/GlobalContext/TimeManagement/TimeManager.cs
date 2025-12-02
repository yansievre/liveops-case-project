using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ProjectContext.DateTimeSource;
using R3;
using Zenject;

namespace ProjectContext
{
    public class TimeManager : IInitializable, IDisposable, ITimeManager
    {
        private readonly IDateTimeSource _dateTimeSource;
        private ReactiveProperty<DateTime> _currentUtcTime = new ReactiveProperty<DateTime>();
        private CancellationTokenSource _cts;
            
        public ReadOnlyReactiveProperty<DateTime> CurrentUtcTime => _currentUtcTime;
        public ReadOnlyReactiveProperty<DateTime> CurrentLocalTime { get; }

        public TimeManager(IDateTimeSource dateTimeSource)
        {
            _dateTimeSource = dateTimeSource;
            CurrentLocalTime = _currentUtcTime.Select(utcTime => utcTime.ToLocalTime()).ToReadOnlyReactiveProperty();
        }
        
        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            TimeLoopAsync().Forget();
        }

        private async UniTaskVoid TimeLoopAsync()
        {
            var previousUtcTime = _dateTimeSource.UtcNow;

            while (true)
            {
                var currentUtcTime = _dateTimeSource.UtcNow;
                if (currentUtcTime != previousUtcTime)
                {
                    _currentUtcTime.Value = currentUtcTime;
                    previousUtcTime = currentUtcTime;
                }
                try
                {
                    // performing this loop once per frame to be as close as possible to real second changes
                    await UniTask.NextFrame(_cts.Token, true);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        
        public void Dispose()
        {
            _currentUtcTime.Dispose();
            CurrentLocalTime.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}