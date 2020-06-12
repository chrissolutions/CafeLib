using CafeLib.Mobile.iOS.Renderers;
using CafeLib.Mobile.Views;
using NavigationOverride.iOS.Runtime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]

namespace CafeLib.Mobile.iOS.Renderers
{
    public class NavigationPageRenderer : PageRenderer
    {
        internal static void Initialize()
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!(Element is ISoftNavigationPage page)) return;

            var root = NavigationController.TopViewController;
            var backButton = root.NavigationItem.LeftBarButtonItem;

            var title = "<" + (string.IsNullOrEmpty(NavigationPage.GetBackButtonTitle(Element))
                            ? "Back"
                            : NavigationPage.GetBackButtonTitle(Element));

            root.NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    if (!page.OnSoftBackButtonPressed())
                    {
                        Messaging.void_objc_msgSend(backButton.Target.Handle, backButton.Action.Handle);
                    }
                }), true);
        }
    }
}