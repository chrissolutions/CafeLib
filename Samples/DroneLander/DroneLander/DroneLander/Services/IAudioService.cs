﻿using System;
// ReSharper disable UnusedMemberInSuper.Global

namespace DroneLander.Services
{
    public interface IAudioService : IServiceProvider
    {
        void AdjustVolume(double volume);
        void KillEngine();
        void ToggleEngine();
        Action OnFinishedPlaying { get; set; }
    }
}