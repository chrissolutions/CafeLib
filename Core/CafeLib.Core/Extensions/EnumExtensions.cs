using System;
using System.Collections.Generic;
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
        /// <returns>array of enum field names</returns>
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
            return fieldInfo?.GetCustomAttribute<EnumMemberAttribute>()?.Value;
        }

        /// <summary>
        /// Return enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns the descriptor value associated to the enum field. 
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>enum descriptor </returns>
        public static string GetDescriptor(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return GetFieldDescriptor(fieldInfo);
        }

        /// <summary>
        /// Returns the enum descriptor values.
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <returns>array of enum descriptor values</returns>
        public static string[] GetDescriptors<T>() where T : Enum
        {
            return typeof(T).GetEnumNames().Select(x => GetFieldDescriptor(typeof(T).GetField(x))).ToArray();
        }

        /// <summary>
        /// Get the descriptor value associated to the enum field.
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static string GetFieldDescriptor(MemberInfo fieldInfo)
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

