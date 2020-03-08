using CafeLib.Mobile.Views;
using Xamarin.Forms;
using View = Android.Views.View;

namespace CafeLib.Mobile.Android.Renderers
{
    internal class NavigationClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly NavigationPage _navigationPage;

        public NavigationClickListener(NavigationPage navigationPage)
        {
            _navigationPage = navigationPage;
        }

        public void OnClick(View v)
        {
            if (_navigationPage.CurrentPage is ISoftNavigationPage page && page.OnSoftBackButtonPressed())
            {
                return;
            }

            _navigationPage?.PopAsync();
        }
    }
}