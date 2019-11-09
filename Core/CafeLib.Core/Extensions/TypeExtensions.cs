using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <param name="_">type object</param>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>(this Type _)
        {
            return typeof(T).CreateInstance<T>(null);
        }

        /// <summary>
        /// Create instance of specified type.
        /// </summary>
        /// <typeparam name="T">type to create</typeparam>
        /// <param name="_">type object</param>
        /// <param name="args">constructor arguments</param>
        /// <returns>instance object</returns>
        public static T CreateInstance<T>(this Type _, params object[] args)
        {
            var activator = FindConstructor(typeof(T), args);
            return (T)activator?.Invoke(args);
        }

        /// <summary>
        /// Gets the default constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns>
        ///     Default constructor if found otherwise null
        /// </returns>
        public static ConstructorInfo FindConstructor(this Type type, params object[] args)
        {
            return args != null && args.Length > 0
                ? type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(MatchSignature)
                : type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(c => !c.GetParameters().Any());

            bool MatchSignature(ConstructorInfo constructorInfo)
            {
                var parameters = constructorInfo.GetParameters();
                if (!parameters.Any()) return false;
                var match = true;
                parameters.ForEach((p, i) => match &= p.ParameterType.IsInstanceOfType(args[i]));
                return match;
            }
        }

        /// <summary>
        /// Determines anonymous type.
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>true if anonymous; false otherwise</returns>
        public static bool IsAnonymousType(this Type type)
        {
            var hasCompilerGeneratedAttribute = type.GetTypeInfo().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName?.Contains("AnonymousType") ?? false;
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;
            return isAnonymousType;
        }

        /// <summary>
        /// Default value comparison for value types.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns>true if equal value type default value otherwise false</returns>
        public static bool IsDefault<T>(this T value) where T : struct
        {
            var isDefault = value.Equals(default(T));
            return isDefault;
        }

        /// <summary>
        /// Gets an assembly creatable type information.
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <returns>enumerable list of type information</returns>
        public static IEnumerable<TypeInfo> CreatableTypes(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Select(t => t.GetTypeInfo())
                .Where(t => !t.IsAbstract)
                .Where(t => t.DeclaredConstructors.Any(c => !c.IsStatic && c.IsPublic))
                .Select(t => t);
        }

        /// <summary>
        /// Get assembly types.
        /// </summary>
        /// <param name="assembly">assembly</param>
        /// <returns>enumerable list of assembly types</returns>
        public static IEnumerable<TypeInfo> GetTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException)
            {
                return new List<TypeInfo>();
            }
        }

        /// <summary>
        /// Get derive types.
        /// </summary>
        /// <param name="types">list of types</param>
        /// <param name="baseType">base type</param>
        /// <returns>enumerable list of derived types</returns>
        public static IEnumerable<TypeInfo> Inherits(this IEnumerable<TypeInfo> types, TypeInfo baseType)
        {
            return types.Where(baseType.IsAssignableFrom);
        }

        /// <summary>
        /// Get derived types
        /// </summary>
        /// <typeparam name="TBase">base type</typeparam>
        /// <param name="types">list of types</param>
        /// <returns>enumerable list of derived types</returns>
        public static IEnumerable<TypeInfo> Inherits<TBase>(this IEnumerable<TypeInfo> types)
        {
            return types.Inherits(typeof(TBase).GetTypeInfo());
        }

        /// <summary>
        /// Find types ending with matching string.
        /// </summary>
        /// <param name="types">list of types</param>
        /// <param name="endsWith">matching string</param>
        /// <returns>enumerable list of types ending with matching string</returns>
        public static IEnumerable<TypeInfo> EndsWith(this IEnumerable<TypeInfo> types, string endsWith)
        {
            return types.Where(x => x.Name.EndsWith(endsWith));
        }


        /// <summary>
        /// Convert object properties to an object map.
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="instance"></param>
        /// <returns>object map</returns>
        public static IDictionary<string, object> ToObjectMap<T>(this Type type, T instance)
        {
            var objectMap = new Dictionary<string, object>();
            if (instance != null)
            {
                TypeDescriptor.GetProperties(instance)
                    .OfType<PropertyDescriptor>()
                    .ToList()
                    .ForEach(x => objectMap.Add(x.Name, x.GetValue(instance)));
            }

            return objectMap;
        }

        /// <summary>
        /// Create a delegate from a method.
        /// </summary>
        /// <param name="method">method info</param>
        /// <param name="instance">object instance</param>
        /// <returns>delegate</returns>
        public static Delegate CreateDelegate(this MethodInfo method, object instance)
        {
            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var expressions = parameters.Select(p => (Expression) p).ToArray();
            var call = Expression.Call(Expression.Constant(instance), method, expressions);
            return Expression.Lambda(call, parameters).Compile();
        }
    }
}
