using System;

namespace CafeLib.Core.Mobile.Support
{
    /// <summary>
    /// Subclass resolver.
    /// </summary>
    /// <typeparam name="T">superclass type</typeparam>
    internal abstract class SubclassResolver<T> where T : class
    {
        private T _subclassInstance;
        private readonly Type _subclassType;

        /// <summary>
        /// Subclass resolver constructor.
        /// </summary>
        /// <param name="resolveType"></param>
        protected SubclassResolver(Type resolveType)
        {
            _subclassType = resolveType.IsSubclassOf(typeof(T))
                ? resolveType
                : throw new ArgumentException($"{nameof(resolveType)} type '{resolveType.Name}' is not derived from {typeof(T).Name}.");
        }

        /// <summary>
        /// Resolve the subclass.
        /// </summary>
        /// <returns>subclass instance</returns>
        public object Resolve()
        {
            return _subclassInstance ?? (_subclassInstance = (T)Activator.CreateInstance(_subclassType));
        }
    }
}
