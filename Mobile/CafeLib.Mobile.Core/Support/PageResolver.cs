using System;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.Support
{
    internal sealed class PageResolver : ResolverBase<Page>
    {
        public PageResolver(Type resolveType)
            : base(resolveType)
        {
        }
    }
}
