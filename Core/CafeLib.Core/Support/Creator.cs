﻿using System;
using CafeLib.Core.Extensions;

namespace CafeLib.Core.Support
{
    public static class Creator
    {
        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <param name="_">type object</param>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>()
        {
            return CreateInstance<T>(null);
        }

        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <param name="_">type object</param>
        /// <param name="args">constructor arguments</param>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>(params object[] args)
        {
            var activator = typeof(T).FindConstructor(args);
            return (T)activator?.Invoke(args);
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