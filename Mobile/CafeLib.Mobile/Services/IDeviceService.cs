﻿using System;

namespace CafeLib.Mobile.Services
{
    public interface IDeviceService
    {
        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">action</param>
        void RunOnMainThread(Action action);
    }
}
