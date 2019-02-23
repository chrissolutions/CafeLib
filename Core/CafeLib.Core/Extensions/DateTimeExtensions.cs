using System;

namespace CafeLib.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a date time to universal time string.
        /// </summary>
        /// <param name="dateTime">date time object</param>
        /// <param name="format">output format</param>
        /// <returns>converted universal time string</returns>
        /// <remarks>Uses RFC1123 as the default format</remarks>
        public static string ToUtcString(this DateTime dateTime, string format = "r")
        {
            return dateTime.ToUniversalTime().ToString(format);
        }
    }
}
