using System;

namespace ProjectContext.DateTimeSource
{
    public class RoundedDownDateTimeSource : IDateTimeSource
    {
        public DateTime Now => RoundDown(DateTime.Now);
        public DateTime UtcNow => RoundDown(DateTime.UtcNow);

        private DateTime RoundDown(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }

    }
}