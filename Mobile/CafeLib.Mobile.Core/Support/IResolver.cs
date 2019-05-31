using System;

namespace CafeLib.Mobile.Core.Support
{
    public interface IResolver<in T> where T : class
    {
        Type GetResolveType();

        TU Resolve<TU>() where TU : T;

        object Resolve(Type type);
    }
}
