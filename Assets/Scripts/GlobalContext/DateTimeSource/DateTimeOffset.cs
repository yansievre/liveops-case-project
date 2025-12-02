using System;

namespace ProjectContext.DateTimeSource
{
    public class DateTimeOffset : IDateTimeOffset
    {
        public TimeSpan Offset { get; set; }
    }
}