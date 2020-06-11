using System.Linq;
using Xamarin.Forms;

namespace CafeLib.Mobile.Views
{
    internal class ModalNavigationPage : NavigationPage, INavigableOwner
    {
        /// <summary>
        /// Page owner
        /// </summary>
        public INavigableOwner Owner { get; internal set; }

        /// <summary>
        /// CafeNavigationPage constructor.
        /// </summary>
        /// <param name="root">root page</param>
        public ModalNavigationPage(Page root)
            : base(root)
        {
            SetOwner(root, this);

            var toolbarItem = (root as CafeContentPage)?.ToolbarItems?.FirstOrDefault() as CafeToolbarItem 
                              ?? new CafeToolbarItem {BarBackgroundColor = root.BackgroundColor, BarTextColor = BarTextColor};
            BarBackgroundColor = toolbarItem.BarBackgroundColor;
            BarTextColor = toolbarItem.BarTextColor;

            Pushed += (s, e) =>
            {
                SetOwner(e.Page, this);
            };

            Popped += (s, e) =>
            {
                SetOwner(e.Page, null);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="owner"></param>
        private static void SetOwner(Page page, INavigableOwner owner)
        {
            switch (page)
            {
                case CafeContentPage contentPage:
                    contentPage.Owner = owner;
                    break;

                case CafeMasterDetailPage masterDetailPage:
                    masterDetailPage.Owner = owner;
                    break;

                case ModalNavigationPage navigationPage:
                    navigationPage.Owner = owner;
                    break;
            }
        }
    }
}
