using System;
using System.Reflection;
using Xamarin.Forms;

namespace CafeLib.Mobile.Core.Support
{
    /// <summary>
    /// Page Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PageAttribute : Attribute
    {
        public Type PageType { get; }

        public PageAttribute(Type pageType)
        {
            if (pageType.GetTypeInfo().IsSubclassOf(typeof(Page)))
            {
                PageType = pageType;
            }
            else
            {
                throw new ArgumentException($"{pageType.Name} is not a subclass of Page");
            }
        }
    }
}
