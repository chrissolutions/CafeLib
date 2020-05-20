using System;
using System.Linq;
using Xamarin.Forms;

namespace CafeLib.Mobile.Attributes
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this Page page) where T : Attribute =>
            page.GetType().GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
    }
}