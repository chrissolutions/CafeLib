using Android.Views;
using AndroidX.DrawerLayout.Widget;
using CafeLib.Mobile.Views;
using Xamarin.Forms;
using View = Android.Views.View;

namespace CafeLib.Mobile.Android.Renderers
{
    internal class FlyoutPageMenuClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly NavigationPage _navigationPage;
        private readonly DrawerLayout _layout;

        public FlyoutPageMenuClickListener(NavigationPage navigationPage, DrawerLayout layout)
        {
            _navigationPage = navigationPage;
            _layout = layout;
        }

        public void OnClick(View v)
        {
            if (_navigationPage.Navigation.NavigationStack.Count <= 1)
            {
                _layout.OpenDrawer((int)GravityFlags.Left);
                return;
            }

            if (_navigationPage.CurrentPage is ISoftNavigationPage page && page.OnSoftBackButtonPressed())
            {
                return;
            }

            _navigationPage?.PopAsync();
        }
    }
}