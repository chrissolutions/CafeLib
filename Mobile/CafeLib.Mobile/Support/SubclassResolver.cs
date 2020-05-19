using System;

namespace CafeLib.Mobile.Support
{
    /// <summary>
    /// Subclass resolver.
    /// </summary>
    /// <typeparam name="T">superclass type</typeparam>
    internal abstract class SubclassResolver<T> where T : class
    {
        private T _resolveInstance;
        private readonly Type _resolveType;

        /// <summary>
        /// Subclass resolver constructor.
        /// </summary>
        /// <param name="resolveType"></param>
        protected SubclassResolver(Type resolveType)
        {
            _resolveType = resolveType.IsSubclassOf(typeof(T))
                ? resolveType
                : throw new ArgumentException($"{nameof(resolveType)} type '{resolveType.Name}' is not derived from {typeof(T).Name}.");
        }

        /// <summary>
        /// Resolve the subclass.
        /// </summary>
        /// <returns>subclass instance</returns>
        public object Resolve()
        {
            return _resolveInstance ?? (_resolveInstance = (T)Activator.CreateInstance(_resolveType));
        }

        /// <summary>
        /// Release the resolved instance.
        /// </summary>
        public void Release()
        {
            _resolveInstance = default;
        }
    }
}
