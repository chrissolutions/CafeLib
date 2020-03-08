using System;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a date time to universal time string.
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <param name="format">output format</param>
        /// <returns>converted universal time string</returns>
        /// <remarks>Uses RFC1123 as the default format</remarks>
        public static string ToUtcString(this DateTime dateTime, string format = "r")
        {
            return dateTime.ToUniversalTime().ToString(format);
        }

        /// <summary>
        /// Adds interval to datetime until greater than current time.
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <param name="interval">interval time span</param>
        /// <returns>datetime</returns>
        public static DateTime NextTime(this DateTime dateTime, TimeSpan interval)
        {
            var timespan = dateTime < DateTime.Now 
                ? interval != TimeSpan.Zero ? interval : throw new ArgumentException($"Zero {nameof(interval)} results in endless loop.")
                : TimeSpan.Zero;

            var nextTime = dateTime.Add(timespan);
            while (nextTime < DateTime.Now)
            {
                nextTime = nextTime.Add(timespan);
            }

            return nextTime;
        }
    }
}