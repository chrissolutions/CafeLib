using System;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class DateTimeExtensions
    {
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

        /// <summary>
        /// Adds interval to datetime until greater than current utc time.
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <param name="interval">interval time span</param>
        /// <returns>datetime</returns>
        public static DateTime NextTimeUtc(this DateTime dateTime, TimeSpan interval)
        {
            var timespan = dateTime < DateTime.UtcNow
                ? interval != TimeSpan.Zero ? interval : throw new ArgumentException($"Zero {nameof(interval)} results in endless loop.")
                : TimeSpan.Zero;

            var nextTime = dateTime.Add(timespan);
            while (nextTime < DateTime.UtcNow)
            {
                nextTime = nextTime.Add(timespan);
            }

            return nextTime;
        }

        /// <summary>
        /// Converts datetime to Unix time in milliseconds.
        /// </summary>
        /// <param name="dateTime">date time</param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime dateTime)
        {
            return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
        }

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
        /// Truncates hours from datetime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime TruncateHours(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.TicksPerDay);
        }

        /// <summary>
        /// Truncates minutes from datetime.
        /// </summary>
        /// <param name="dateTime">datetime source.</param>
        /// <returns>truncated datetime</returns>
        public static DateTime TruncateMinutes(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.TicksPerHour);
        }

        /// <summary>
        /// Truncates seconds from dateTime.
        /// </summary>
        /// <param name="dateTime">datetime source.</param>
        /// <returns>truncated datetime</returns>
        public static DateTime TruncateSeconds(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.TicksPerMinute);
        }

        /// <summary>
        /// Truncates milliseconds from dateTime.
        /// </summary>
        /// <param name="dateTime">datetime source.</param>
        /// <returns>truncated datetime</returns>
        public static DateTime TruncateMilliseconds(this DateTime dateTime)
        {
            return dateTime.Truncate(TimeSpan.TicksPerSecond);
        }

        #region Helpers

        /// <summary>
        /// Truncates a DateTime to a specified resolution.
        /// A convenient source for resolution is TimeSpan.TicksPerXXXX constants.
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        private static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        #endregion
    }
}