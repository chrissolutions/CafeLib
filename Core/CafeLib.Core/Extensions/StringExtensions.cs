using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CafeLib.Core.Extensions
{
    public static class StringExtensions
    {
        #region Converters

        public static bool ToBool(this string s)
        {
            return Converter.Convert<bool>(s);
        }

        public static char ToChar(this string s)
        {
            return Converter.Convert<char>(s);
        }

        public static sbyte ToSbyte(this string s)
        {
            return Converter.Convert<sbyte>(s);
        }

        public static byte ToByte(this string s)
        {
            return Converter.Convert<byte>(s);
        }

        public static short ToShort(this string s)
        {
            return Converter.Convert<short>(s);
        }

        public static ushort ToUshort(this string s)
        {
            return Converter.Convert<ushort>(s);
        }

        public static int ToInt(this string s)
        {
            return Converter.Convert<int>(s);
        }

        public static uint ToUint(this string s)
        {
            return Converter.Convert<uint>(s);
        }

        public static long ToLong(this string s)
        {
            return Converter.Convert<long>(s);
        }

        public static ulong ToUlong(this string s)
        {
            return Converter.Convert<ulong>(s);
        }

        public static float ToFloat(this string s)
        {
            return Converter.Convert<float>(s);
        }

        public static double ToDouble(this string s)
        {
            return Converter.Convert<double>(s);
        }

        public static decimal ToDecimal(this string s)
        {
            return Converter.Convert<decimal>(s);
        }

        public static DateTime ToDateTime(this string s)
        {
            return Converter.Convert<DateTime>(s);
        }

        public static Guid ToGuid(this string s)
        {
            return Converter.Convert<Guid>(s);
        }

        #endregion

        #region Render

        /// <summary>
        /// Renders the string using the placeholder for the property names.
        /// </summary>
        /// <param name="format">The string to format.</param>
        /// <param name="values">The object to pull the values from. Usually an anonymous type.</param>
        /// <returns>The rendered string.</returns>
        public static string Render(this string format, object values = null)
        {
            return format.Render(null, values);
        }

        /// <summary>
        /// Renders the string using the placeholder for the property names.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="provider">The format provider used to format dates and numbers.</param>
        /// <param name="values">The object to pull the values from. Usually an anonymous type.</param>
        /// <returns>The rendered string.</returns>
        public static string Render(this string format, IFormatProvider provider, object values = null)
        {
            return format.Render(provider, ToObjectMap(values));
        }

        /// <summary>
        /// Formats the string using the placeholder for the property names.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="values">The dictionary to pull the values from.</param>
        /// <returns>The rendered string.</returns>
        public static string Render(this string format, IDictionary<string, object> values)
        {
            return format.Render(null, values);
        }

        /// <summary>
        /// Renders the string using the placeholder for the property names.
        /// </summary>
        /// <param name="format">The string to format.</param>
        /// <param name="provider">The provider to use for formatting dates and numeric values.</param>
        /// <param name="values">The dictionary to pull the values from.</param>
        /// <returns>The rendered string.</returns>
        public static string Render(this string format, IFormatProvider provider, IDictionary<string, object> values)
        {
            if (values == null) return format;
            var results = ParseFormat(format);
            return string.Format(provider, results.Item1, results.Item2.Select(x => values[x]).ToArray());
        }

        /// <summary>
        /// Parse the format string.
        /// </summary>
        /// <param name="source">source format string</param>
        /// <returns>
        /// A tuple consisting of the altered format string and a symbol table.
        /// </returns>
        /// <remarks>
        /// Parse transition matrix:
        /// |--------------------------------------------------------------------------|
        /// |     | \w  | \d  | \s  |  {  |  }  |  :  | eos |  
        /// |--------------------------------------------------------------------------|
        /// |  0  |  0  |  0  |  0  |  1  |  A  |  0  | exit|  Initial
        /// |--------------------------------------------------------------------------|
        /// |  1  |  2  |  E  |  E  |  0  |  E  |  E  |  E  |  Initial symbol
        /// |--------------------------------------------------------------------------|
        /// |  2  |  2  |  2  |  E  |  E  | B/0 | B/3 |  E  |  Build symbol
        /// |--------------------------------------------------------------------------|
        /// |  3  |  3  |  3  |  3  |  E  | C/0 |  E  |  E  |  Build format parameter
        /// |--------------------------------------------------------------------------|
        /// 
        /// A)	Lookahead.
        ///     If character is '}' Then ++index, goto 0
        ///     Else goto E
        /// 
        /// B)	Symbol found.
        /// 
        /// C)  Format parameter found.
        /// 
        /// E)	Syntax error.
        /// </remarks>
        private static Tuple<string, HashSet<string>> ParseFormat(string source)
        {
            int state = 0;
            int index = 0;
            var target = new StringBuilder();
            var symbol = new StringBuilder();
            var format = new StringBuilder();
            var symbolTable = new HashSet<string>();
            var tagTable = new HashSet<string>();

            while (index < source.Length)
            {
                switch (state)
                {
                    case 0: // Initial
                        if (source[index] == '{')
                        {
                            symbol = new StringBuilder();
                            state = 1;
                        }
                        else if (source[index] == '}')
                        {
                            if (Lookahead(source, index, '}'))
                            {
                                target.Append(source[index++]);
                            }
                            else
                            {
                                throw new FormatException();
                            }
                        }

                        target.Append(source[index]);
                        index += 1;
                        continue;

                    case 1: // Start symbol parse.
                        if (source[index] == '{')
                        {
                            target.Append(source[index++]);
                            state = 0;
                            continue;
                        }
                        if (char.IsLetter(source[index]))
                        {
                            state = 2;
                            symbol.Append(source[index]);
                            index += 1;
                            continue;
                        }
                        throw new FormatException();

                    case 2: // Build symbol
                        if (char.IsLetter(source[index]) || char.IsDigit(source[index]))
                        {
                            state = 2;
                            symbol.Append(source[index]);
                            index += 1;
                            continue;
                        }

                        if (source[index] == '}')
                        {
                            // Process symbol
                            symbolTable.Add(symbol.ToString());
                            tagTable.Add(symbol.ToString());
                            target.Append(GetPlaceholder(symbol.ToString(), tagTable));
                            target.Append(source[index]);
                            state = 0;
                            index += 1;
                            continue;
                        }

                        if (source[index] == ':')
                        {
                            // Start parse of format parameters
                            symbolTable.Add(symbol.ToString());
                            tagTable.Add(symbol.ToString());
                            state = 3;
                            format = new StringBuilder();
                            index += 1;
                            continue;
                        }

                        throw new FormatException();

                    case 3: // Parse format parameters
                        if (source[index] == '}')
                        {
                            // Complete parse of format parameters
                            target.AppendFormat("{0}:{1}", GetPlaceholder(symbolTable.Last(), tagTable), format);
                            target.Append(source[index]);
                            state = 0;
                            index += 1;
                            continue;
                        }

                        format.Append(source[index]);
                        index += 1;
                        continue;

                    default: // Invalid state
                        throw new FormatException();
                }
            }

            if (state != 0)
                throw new FormatException();

            return new Tuple<string, HashSet<string>>(target.ToString(), symbolTable);
        }

        /// <summary>
        /// Lookahead one character.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="index"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        private static bool Lookahead(string format, int index, char character)
        {
            return index < format.Length && format[index + 1] == character;
        }

        /// <summary>
        /// Converts anonymous object properties to an object map.
        /// </summary>
        /// <param name="anonymousObject"></param>
        /// <returns>object map</returns>
        private static IDictionary<string, object> ToObjectMap(object anonymousObject)
        {
            var objectMap = new Dictionary<string, object>();
            if (anonymousObject != null)
            {
                TypeDescriptor.GetProperties(anonymousObject)
                    .OfType<PropertyDescriptor>()
                    .ToList()
                    .ForEach(x => objectMap.Add(x.Name, x.GetValue(anonymousObject)));
            }

            return objectMap;
        }

        /// <summary>
        /// Get the format placeholder id.
        /// </summary>
        /// <param name="symbol">symbol</param>
        /// <param name="tagTable">symbol table</param>
        /// <returns>format placeholder id</returns>
        private static int GetPlaceholder(string symbol, ICollection<string> tagTable)
        {
            var index = tagTable.ToList().FindIndex(x => x == symbol);
            return index != -1 ? index : tagTable.Count;
        }

        #endregion
    }
}
