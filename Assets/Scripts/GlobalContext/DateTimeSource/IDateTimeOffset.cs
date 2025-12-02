#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;

namespace ProjectContext.DateTimeSource
{
    public interface IDateTimeOffset
    {
        TimeSpan Offset { get; }
    }
}
#endif