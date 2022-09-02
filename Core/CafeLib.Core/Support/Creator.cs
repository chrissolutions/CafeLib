using System;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Support
{
    public static class Creator
    {
        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>()
        {
            return CreateInstance<T>(null);
        }

        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <param name="args">constructor arguments</param>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <param name="type">type object</param>
        /// <param name="args">constructor arguments</param>
        /// <returns>instance object</returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            var activator = type.FindConstructor(args);
            return activator?.Invoke(args);
        }
    }
}
