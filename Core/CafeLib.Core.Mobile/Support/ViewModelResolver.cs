using System;
using CafeLib.Core.Mobile.ViewModels;

namespace CafeLib.Core.Mobile.Support
{
    internal sealed class ViewModelResolver : ResolverBase<BaseViewModel>
    {
        public ViewModelResolver(Type resolveType)
            : base(resolveType)
        {
        }
    }
}
