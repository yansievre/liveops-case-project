#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;

namespace ProjectContext.DateTimeSource
{
    public class DateTimeSourceOffsetDecorator : IDateTimeSource
    {
        private readonly IDateTimeSource _dateTimeSource;
        private readonly IDateTimeOffset _dateTimeOffset;
        public DateTime Now => _dateTimeSource.Now.Add(_dateTimeOffset.Offset);
        public DateTime UtcNow => _dateTimeSource.UtcNow.Add(_dateTimeOffset.Offset);

        public DateTimeSourceOffsetDecorator(IDateTimeSource dateTimeSource, IDateTimeOffset dateTimeOffset)
        {
            _dateTimeSource = dateTimeSource;
            _dateTimeOffset = dateTimeOffset;
        }
    }
}
#endif