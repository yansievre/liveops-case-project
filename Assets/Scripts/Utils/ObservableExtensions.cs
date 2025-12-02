using System;
using R3;

namespace Utils
{
    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this Observable<T> source, Action onNext)
        {
            return source.Subscribe(_ => onNext());
        }

    }
}