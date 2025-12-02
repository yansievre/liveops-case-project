using System;

namespace Utils
{
    public static class DateTimeUtil
    {
        
        public static string FormatTimeSpan(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
            {
                return $"{timeSpan.Days}d {timeSpan.Hours}h";
            }
            if (timeSpan.TotalHours >= 1)
            {
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
            }
            if (timeSpan.TotalMinutes >= 1)
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            return $"{timeSpan.Seconds}s";
        }

    }
}