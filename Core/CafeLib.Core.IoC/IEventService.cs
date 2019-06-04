using System;
using CafeLib.Core.Eventing;

namespace CafeLib.Core.IoC
{
    public interface IEventService
    {
        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name='action'>
        /// Action.
        /// </param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        Guid Subscribe<T>(Action<T> action) where T : IEventMessage;

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        void Publish<T>(T message) where T : IEventMessage;

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        void Unsubscribe<T>() where T : IEventMessage;

        /// <summary>
        /// Unsubscribe the specified handler of type T and Guid identifier.
        /// </summary>
        /// <param name="actionId">action id</param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        void Unsubscribe<T>(Guid actionId) where T : IEventMessage;
    }
}
