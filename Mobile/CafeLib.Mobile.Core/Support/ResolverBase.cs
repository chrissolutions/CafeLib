using System;

namespace CafeLib.Mobile.Core.Support
{
    internal abstract class ResolverBase<T> : IResolver<T> where T : class
    {
        private T _resolveObj;
        private readonly Type _resolveType;

        protected ResolverBase(Type resolveType)
        {
            _resolveType = resolveType;
        }

        public Type GetResolveType()
        {
            return _resolveType;
        }

        public object Resolve(Type type)
        {
            return _resolveObj ?? (_resolveObj = (T) Activator.CreateInstance(type));
        }

        public TU Resolve<TU>() where TU : T
        {
            return (TU) Resolve(typeof(TU));
        }
    }
}
