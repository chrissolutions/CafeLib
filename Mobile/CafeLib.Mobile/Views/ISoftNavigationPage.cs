// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public interface ISoftNavigationPage
    {
        /// <summary>
        /// Process software back button press event.
        /// </summary>
        /// <returns>true: ignore behavior; false: default behavior</returns>
        bool OnSoftBackButtonPressed();
    }
}