using System;
using Android.Media;
using DroneLander.Services;

namespace DroneLander.Droid.Services
{
    public class AudioService : IAudioService
    {
        private const float DefaultVolume = 0.1f;

        private MediaPlayer _mediaPlayer;

        public Action OnFinishedPlaying { get; set; }

        public void AdjustVolume(double level)
        {
            float volume = (float)(level / 100.0);
            if (volume.Equals(0.0f)) volume = DefaultVolume;
            _mediaPlayer.SetVolume(volume, volume);
        }

        public void KillEngine()
        {
            _mediaPlayer?.SetVolume(0.0f, 0.0f);
        }

        public void ToggleEngine()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Completion -= OnMediaCompleted;
                _mediaPlayer.Stop();
                _mediaPlayer = null;
            }
            else
            {
                const string fullPath = "Sounds/engine.m4a";
                Android.Content.Res.AssetFileDescriptor afd = null;

                try
                {
                    afd = Android.App.Application.Context.Assets.OpenFd(fullPath);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (afd != null)
                {
                    if (_mediaPlayer == null)
                    {
                        _mediaPlayer = new MediaPlayer();
                        _mediaPlayer.Prepared += (sender, args) =>
                        {
                            _mediaPlayer.Start();
                            _mediaPlayer.Completion += OnMediaCompleted;
                        };
                    }

                    _mediaPlayer.Reset();
                    _mediaPlayer.SetVolume(DefaultVolume, DefaultVolume);
                    _mediaPlayer.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                    _mediaPlayer.PrepareAsync();
                }
            }
        }

        private void OnMediaCompleted(object sender, EventArgs e)
        {
            OnFinishedPlaying?.Invoke();
        }
    }
}