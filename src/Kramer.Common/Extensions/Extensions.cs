using System;

namespace Kramer.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToSwedishDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToSwedishTime(this DateTime date)
        {
            return date.ToString("HH:mm");
        }

        public static string ToMinSecString(this int durationInSecs)
        {
            return string.Format("{0} min", 
                TimeSpan.FromSeconds(durationInSecs).ToString(@"mm\:ss"));
        }
    }
}