using CafeLib.Core.Mobile.Services;

namespace DroneLander.Services
{
    public static class AudioHelper
    {
        public static void ToggleEngine()
        {
            var audioPlayer = MobileServices.Resolve<IAudioService>();
            audioPlayer.ToggleEngine();
        }

        public static void AdjustVolume(double volume)
        {
            var audioPlayer = MobileServices.Resolve<IAudioService>();
            audioPlayer.AdjustVolume(volume);
        }

        public static void KillEngine()
        {
            var audioPlayer = MobileServices.Resolve<IAudioService>();
            audioPlayer.KillEngine();
        }
    }
}