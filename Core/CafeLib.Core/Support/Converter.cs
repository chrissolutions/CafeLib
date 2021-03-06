using System;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    public static class Converter
    {
        #region Instrinsic Types Converter Map

        /// <summary>
        /// Map of instrinsic data types.
        /// </summary>
        private static readonly Dictionary<Type, Converter<string, object>> _stringConverterMap =
            new Dictionary<Type, Converter<string, object>>
            {
                {typeof(bool), s=> ToBool(s)},
                {typeof(char), s => ToChar(s)},
                {typeof(sbyte), s => ToSbyte(s)},
                {typeof(byte), s => ToByte(s)},
                {typeof(short), s => ToShort(s)},
                {typeof(ushort), s => ToUshort(s)},
                {typeof(int), s => ToInt(s)},
                {typeof(uint), s => ToUint(s)},
                {typeof(long), s => ToLong(s)},
                {typeof(ulong), s => ToUlong(s)},
                {typeof(float), s => ToFloat(s)},
                {typeof(double), s => ToDouble(s)},
                {typeof(decimal), s => ToDecimal(s)},
                {typeof(DateTime), s => ToDateTime(s)},
                {typeof(Guid), s => ToGuid(s)},
                {typeof(string), s => s}
            };

        private static readonly Dictionary<Type, Converter<object, object>> _objectConverterMap =
            new Dictionary<Type, Converter<object, object>>
            {
                {typeof(bool), o => System.Convert.ToBoolean(o)},
                {typeof(char), o => System.Convert.ToChar(o)},
                {typeof(sbyte), o => System.Convert.ToSByte(o)},
                {typeof(byte), o => System.Convert.ToByte(o)},
                {typeof(short), o => System.Convert.ToInt16(o)},
                {typeof(ushort), o => System.Convert.ToUInt16(o)},
                {typeof(int), o => System.Convert.ToInt32(o)},
                {typeof(uint), o => System.Convert.ToUInt32(o)},
                {typeof(long), o => System.Convert.ToInt64(o)},
                {typeof(ulong), o => System.Convert.ToUInt64(o)},
                {typeof(float), o => System.Convert.ToSingle(o)},
                {typeof(double), o => System.Convert.ToDouble(o)},
                {typeof(decimal), o => System.Convert.ToDecimal(o)},
                {typeof(DateTime), o => System.Convert.ToDateTime(o)},
                {typeof(Guid), o => ToGuid(o.ToString())},
                {typeof(string), o => o.ToString()}
            };

        #endregion

        /// <summary>
        /// Convert a string to an instrisic type.
        /// </summary>
        /// <typeparam name="T">conversion type</typeparam>
        /// <param name="value">string value</param>
        /// <returns>converted value</returns>
        public static T Convert<T>(string value)
        {
            return (T) ConvertTo(typeof(T), value);
        }

        /// <summary>
        /// Converts an object to an intrinsic type.
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="value">object value</param>
        /// <returns></returns>
        public static object ConvertTo(Type type, string value)
        {
            return _stringConverterMap[type](value);
        }

        /// <summary>
        /// Converts an object to an intrinsic type.
        /// </summary>
        /// <typeparam name="T">conversion type</typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="value">object value</param>
        /// <returns></returns>
        public static T Convert<T, TU>(TU value) where TU : class
        {
            return (T)ConvertTo(typeof(T), value);
        }

        /// <summary>
        /// Converts an object to an intrinsic type.
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="value">object value</param>
        /// <returns>converted value or original object</returns>
        public static object ConvertTo<T>(Type type, T value) where T : class
        {
            try
            {
                return _objectConverterMap[type](value);
            }
            catch
            {
                return value;
            }
        }

        #region Helpers

        private static bool ToBool(string s)
        {
            return bool.TryParse(s, out var result) && result;
        }

        private static char ToChar(string s)
        {
            return char.TryParse(s, out var result) ? result : default;
        }

        private static sbyte ToSbyte(string s)
        {
            return sbyte.TryParse(s, out var result) ? result : default;
        }

        private static byte ToByte(string s)
        {
            return byte.TryParse(s, out var result) ? result : default;
        }

        private static short ToShort(string s)
        {
            return short.TryParse(s, out var result) ? result : default;
        }

        private static ushort ToUshort(string s)
        {
            return ushort.TryParse(s, out var result) ? result : default;
        }

        private static int ToInt(string s)
        {
            return int.TryParse(s, out var result) ? result : default;
        }

        private static uint ToUint(string s)
        {
            return uint.TryParse(s, out var result) ? result : default;
        }
        private static long ToLong(string s)
        {
            return long.TryParse(s, out var result) ? result : default;
        }

        private static ulong ToUlong(string s)
        {
            return ulong.TryParse(s, out var result) ? result : default;
        }

        private static float ToFloat(string s)
        {
            return float.TryParse(s, out var result) ? result : default;
        }

        private static double ToDouble(string s)
        {
            return double.TryParse(s, out var result) ? result : default;
        }

        private static decimal ToDecimal(string s)
        {
            return decimal.TryParse(s, out var result) ? result : default;
        }

        private static DateTime ToDateTime(string s)
        {
            return DateTime.TryParse(s, out var result) ? result : default;
        }

        private static Guid ToGuid(string s)
        {
            return Guid.TryParse(s, out var result) ? result : default;
        }

        #endregion
    }
}
