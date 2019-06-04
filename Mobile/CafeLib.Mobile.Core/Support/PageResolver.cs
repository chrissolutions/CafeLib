using System;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.Support
{
    internal sealed class PageResolver : SubclassResolver<Page> //: ResolverBase<Page>
    {
        /// <summary>
        /// Page resolver constructor
        /// </summary>
        /// <param name="resolveType"></param>
        public PageResolver(Type resolveType)
            : base(resolveType)
        {
        }
    }
}
