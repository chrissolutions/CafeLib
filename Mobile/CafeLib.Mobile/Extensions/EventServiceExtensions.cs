using System;
using CafeLib.Core.Eventing;
using Xamarin.Forms;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Extensions
{
    public static class EventServiceExtensions
    {
        /// <summary>
        /// Subscribe to event message with action to run on the main thread
        /// </summary>
        /// <typeparam name="T">event message type</typeparam>
        /// <param name="eventService"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Guid SubscribeOnMainThread<T>(this IEventService eventService, Action<T> action) where T : IEventMessage
        {
            return eventService.Subscribe<T>(x => Application.Current.RunOnMainThread(() => action(x)));
        }

        /// <summary>
        /// Subscribe to event message with action to run on a worker thread
        /// </summary>
        /// <typeparam name="T">event message type</typeparam>
        /// <param name="eventService"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Guid SubscribeOnWorkerThread<T>(this IEventService eventService, Action<T> action) where T : IEventMessage
        {
            return eventService.Subscribe<T>(x => Application.Current.RunOnWorkerThread(() => action(x)));
        }
    }
}
