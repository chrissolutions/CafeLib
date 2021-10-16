using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.Core.IoC
{
    public static class ServiceResolverExtensions
    {
        /// <summary>
        /// Resolve the service.
        /// </summary>
        /// <param name="resolver">resolver</param>
        /// <param name="type">type of service to resolve</param>
        /// <returns></returns>
        public static object Resolve(this IServiceResolver resolver, Type type)
        {
            var resolveMethod = resolver.GetType().GetMethod("Resolve") ?? throw new MissingMemberException(nameof(resolver));
            var method = resolveMethod.MakeGenericMethod(type);
            return method.Invoke(resolver, null);
        }

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <typeparam name="T">service type</typeparam>
        /// <returns>the service object</returns>
        public static bool TryResolve<T>(this IServiceResolver resolver, out T value) where T : class
        {
            try
            {
                value = resolver.Resolve<T>();
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Resolve the specified service type.
        /// </summary>
        /// <returns>the service object</returns>
        public static bool TryResolve(this IServiceResolver resolver, Type type, out object value)
        {
            try
            {
                value = resolver.Resolve(type);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
