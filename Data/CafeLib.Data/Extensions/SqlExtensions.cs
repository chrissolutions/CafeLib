﻿namespace CafeLib.Data.Extensions
{
    public static class SqlExtensions
    {
        public static string SqlEscape(this string parameter)
        {
            return !string.IsNullOrWhiteSpace(parameter) ? parameter.Replace("'", "''") : parameter;
        }
    }
}
