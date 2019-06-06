using System;
using Xamarin.Forms;

namespace CafeLib.Mobile.Support
{
    internal sealed class PageResolver : SubclassResolver<Page>
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
