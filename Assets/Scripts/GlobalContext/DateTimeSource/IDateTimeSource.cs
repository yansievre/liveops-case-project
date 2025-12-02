using System;

namespace ProjectContext.DateTimeSource
{
    public interface IDateTimeSource
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}