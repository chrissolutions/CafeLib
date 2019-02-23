using System;
using Xamarin.Forms;

namespace CafeLib.Core.Mobile.Support
{
    internal sealed class PageResolver : ResolverBase<Page>
    {
        public PageResolver(Type resolveType)
            : base(resolveType)
        {
        }
    }
}
