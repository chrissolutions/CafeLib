using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Views
{
    public interface INavigableOwner
    {
        /// <summary>
        /// Owner page.
        /// </summary>
        INavigableOwner Owner { get; }
    }
}
