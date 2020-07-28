using System.Collections.Generic;
using Newtonsoft.Json.Linq;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Dynamic
{
    public static class Extensions
    {
        /// <summary>
        /// Convert dictionary to an object.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <returns>object</returns>
        public static T ToObject<T>(this IDictionary<string, T> dictionary) where T : class
        {
            return dictionary.ToObject<T, T>();
        }

        /// <summary>
        /// Convert dictionary to an object.
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="dictionary"></param>
        /// <returns>object</returns>
        public static T ToObject<T>(this IDictionary<string, object> dictionary) where T : class
        {
            return dictionary.ToObject<T, object>();
        }

        /// <summary>
        /// Convert dictionary to an object.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns>object</returns>
        public static TObject ToObject<TObject, TValue>(this IDictionary<string, TValue> dictionary) where TObject : class
        {
            var json = JObject.FromObject(dictionary);
            return json.ToObject<TObject>();
        }
    }
}

