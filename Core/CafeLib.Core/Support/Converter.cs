using System;
using System.Collections.Generic;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    public static class Converter
    {
        #region Instrinsic Types Converter Maps

        /// <summary>
        /// Map of intrinsic data types.
        /// </summary>
        private static readonly Dictionary<Type, Converter<string, object>> StringConverterMap =
            new()
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

        private static readonly Dictionary<Type, Converter<object, object>> ObjectConverterMap =
            new()
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

        #region Methods

        /// <summary>
        /// Convert a string value to an intrinsic type.
        /// </summary>
        /// <typeparam name="T">conversion type</typeparam>
        /// <param name="value">string value</param>
        /// <returns>converted value</returns>
        public static T Convert<T>(string value) => (T) ConvertTo(typeof(T), value);

        /// <summary>
        /// Converts a string value to an intrinsic type.
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="value">object value</param>
        /// <returns></returns>
        public static object ConvertTo(Type type, string value) => StringConverterMap[type](value);

        /// <summary>
        /// Converts an object to an intrinsic type.
        /// </summary>
        /// <typeparam name="T">conversion type</typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="value">object value</param>
        /// <returns></returns>
        public static T Convert<T, TU>(TU value) where TU : class => (T)ConvertTo(typeof(T), value);

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
                return ObjectConverterMap[type](value);
            }
            catch
            {
                return value;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Convert string value to boolean
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>boolean value</returns>
        private static bool ToBool(string s) => 
            bool.TryParse(s, out var result) && result;

        /// <summary>
        /// Convert string value to char
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>char value</returns>
        private static char ToChar(string s) => 
            char.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to signed byte
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>sbyte value</returns>
        private static sbyte ToSbyte(string s) => 
            sbyte.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to byte
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>byte value</returns>
        private static byte ToByte(string s) => 
            byte.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to short
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>short value</returns>
        private static short ToShort(string s) => 
            short.TryParse(s, out var result) ? result : default;
        
        /// <summary>
        /// Convert string value to unsigned short
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>unsigned short value</returns>
        private static ushort ToUshort(string s) => 
            ushort.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to int
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>int value</returns>
        private static int ToInt(string s) => 
            int.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to unsigned int
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>unsigned int value</returns>
        private static uint ToUint(string s) => 
            uint.TryParse(s, out var result) ? result : default;
    
        /// <summary>
        /// Convert string value to long
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>long value</returns>
        private static long ToLong(string s) => 
            long.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to unsigned long
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>unsigned long value</returns>
        private static ulong ToUlong(string s) => 
            ulong.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to float
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>float value</returns>
        private static float ToFloat(string s) =>
            float.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to double
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>double value</returns>
        private static double ToDouble(string s) =>
            double.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to decimal
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>decimal value</returns>
        private static decimal ToDecimal(string s) => 
            decimal.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to DateTime
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>DateTime value</returns>
        private static DateTime ToDateTime(string s) =>
            DateTime.TryParse(s, out var result) ? result : default;

        /// <summary>
        /// Convert string value to Guid
        /// </summary>
        /// <param name="s">string value</param>
        /// <returns>Guid value</returns>
        private static Guid ToGuid(string s) =>
            Guid.TryParse(s, out var result) ? result : default;

        #endregion
    }
}
