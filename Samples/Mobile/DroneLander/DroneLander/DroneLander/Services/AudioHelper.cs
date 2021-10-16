using CafeLib.Mobile.Extensions;
using Xamarin.Forms;

namespace DroneLander.Services
{
    public static class AudioHelper
    {
        public static void ToggleEngine()
        {
            var audioPlayer = Application.Current.Resolve<IAudioService>();
            audioPlayer.ToggleEngine();
        }

        public static void AdjustVolume(double volume)
        {
            var audioPlayer = Application.Current.Resolve<IAudioService>();
            audioPlayer.AdjustVolume(volume);
        }

        public static void KillEngine()
        {
            var audioPlayer = Application.Current.Resolve<IAudioService>();
            audioPlayer.KillEngine();
        }
    }
}