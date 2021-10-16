using System.Linq;
using Xamarin.Forms;

namespace CafeLib.Mobile.Views
{
    internal class ModalNavigationPage : NavigationPage
    {
        /// <summary>
        /// CafeNavigationPage constructor.
        /// </summary>
        /// <param name="root">root page</param>
        public ModalNavigationPage(Page root)
            : base(root)
        {
            var toolbarItem = (root as CafeContentPage)?.ToolbarItems?.FirstOrDefault() as CafeToolbarItem 
                              ?? new CafeToolbarItem {BarBackgroundColor = root.BackgroundColor, BarTextColor = BarTextColor};
            BarBackgroundColor = toolbarItem.BarBackgroundColor;
            BarTextColor = toolbarItem.BarTextColor;
        }
    }
}
