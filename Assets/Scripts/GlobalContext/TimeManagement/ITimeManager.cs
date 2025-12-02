using System;
using R3;

namespace ProjectContext
{
    public interface ITimeManager
    {
        ReadOnlyReactiveProperty<DateTime> CurrentUtcTime { get; }
        ReadOnlyReactiveProperty<DateTime> CurrentLocalTime { get; }
    }
}