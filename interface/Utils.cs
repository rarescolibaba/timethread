using System;

namespace @interface
{
    public static class Utils
    {
        /// <summary>
        /// Formateaza un TimeSpan ca ore si minute
        /// </summary>
        /// <param name="time">TimeSpan to format</param>
        /// <returns>Formatted time string</returns>
        public static string FormatTimeSpan(TimeSpan time)
        {
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        }
    }
}