using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KramerUwp.App
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
            return TimeSpan.FromSeconds(durationInSecs).ToString(@"mm\:ss");
        }
    }
}
