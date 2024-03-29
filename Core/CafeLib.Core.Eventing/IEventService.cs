using CafeLib.Core.Eventing.Subscribers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Eventing
{
    public interface IEventService
    {
        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name='action'>action</param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        Guid Subscribe<T>(Action<T> action) where T : IEventMessage;

        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <param name="operation">task operation</param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        public Guid Subscribe<T>(Func<T, Task> operation) where T : IEventMessage;

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
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>Message.</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Task PublishAsync<T>(T message) where T : IEventMessage;

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
        /// <param name="subscriberId">subscriber identifier</param>
        /// <typeparam name='T'>
        /// Event message type parameter.
        /// </typeparam>
        void Unsubscribe<T>(Guid subscriberId) where T : IEventMessage;

        /// <summary>
        /// Unsubscribe the specified handler using subscriber identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        void Unsubscribe(Guid subscriberId);
    }
}
