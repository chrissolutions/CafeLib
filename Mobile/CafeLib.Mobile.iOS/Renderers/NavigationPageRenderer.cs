using CafeLib.Mobile.iOS.Renderers;
using CafeLib.Mobile.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]

namespace CafeLib.Mobile.iOS.Renderers
{
    public class NavigationPageRenderer : NavigationRenderer
    {
        private const string BackTitle = "Back";

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!(Element is ModalNavigationPage element)) return;
            if (!(element.RootPage is ISoftNavigationPage page)) return;

            var title = $"<{(string.IsNullOrEmpty(NavigationPage.GetBackButtonTitle(Element)) ? BackTitle : NavigationPage.GetBackButtonTitle(Element))}";

            TopViewController.NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(title, UIBarButtonItemStyle.Plain, (sender, args) =>
                {
                    if (!page.OnSoftBackButtonPressed())
                    {
                        element.PopAsync(animated);
                    }
                }), animated);
        }
    }
}