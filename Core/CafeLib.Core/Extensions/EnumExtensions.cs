using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the name of enum field.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Get enum field names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetNames<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T));
        }

        /// <summary>
        /// Get value associated to the EnumMemberAttribute
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>EnumMemberAttribute value</returns>
        public static string GetEnumMemberValue(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return fieldInfo.GetCustomAttribute<EnumMemberAttribute>()?.Value;
        }

        /// <summary>
        /// Humanize the enum field.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Humanize(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return GetDescriptor(fieldInfo);
        }

        /// <summary>
        /// Obtains the enum descriptor values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>array of enum descriptor values</returns>
        public static string[] GetDescriptors<T>() where T : Enum
        {
            return typeof(T).GetEnumNames().Select(x => GetDescriptor(typeof(T).GetField(x))).ToArray();
        }

        /// <summary>
        /// Get the descriptor value associated with the enum field.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetDescriptor(MemberInfo fieldInfo)
        {
            var attr = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
            if (attr != null)
            {
                return attr.Description;
            }

            const string pattern = @"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])";
            var regex = new Regex(pattern);
            return regex.Replace(fieldInfo.Name, " ");
        }
    }
}
