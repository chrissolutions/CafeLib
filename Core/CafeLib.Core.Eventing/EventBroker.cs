using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Eventing
{
    public class EventBroker : IEventBroker, IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Event service.
        /// </summary>
        private readonly EventService _eventService;

        /// <summary>
        /// Event broker name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        internal EventBroker(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _eventService = new EventService();
        }

        /// <summary>
        /// Subscribe the specified handler.
        /// </summary>
        /// <typeparam name='T'>Type of IEventMessage.</typeparam>
        /// <param name='action'>Event action.</param>
        /// <returns>subscriber id</returns>
        public Guid Subscribe<T>(Action<T> action) where T : IEventMessage
        {
            return _eventService.Subscribe(action);
        }

        /// <summary>
        /// Subscribe the specified async handler.
        /// </summary>
        /// <typeparam name='T'>Type of IEventMessage.</typeparam>
        /// <param name="operation">Event operation.</param>
        /// <returns>subscriber id</returns>
        public Guid Subscribe<T>(Func<T, Task> operation) where T : IEventMessage
        {
            return _eventService.Subscribe(operation);
        }

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Publish<T>(T message) where T : IEventMessage
        {
            _eventService.Publish(message);
        }

        /// <summary>
        /// Publish the specified message.
        /// </summary>
        /// <param name='message'>
        /// Message.
        /// </param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public Task PublishAsync<T>(T message) where T : IEventMessage
        {
            return _eventService.PublishAsync(message);
        }

        /// <summary>
        /// Unsubscribe all specified handlers of type T.
        /// </summary>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>() where T : IEventMessage
        {
            _eventService.Unsubscribe<T>();
        }

        /// <summary>
        /// Unsubscribe the specified handler of type T and Guid identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        /// <typeparam name='T'>
        /// Type of IEventMessage.
        /// </typeparam>
        public void Unsubscribe<T>(Guid subscriberId) where T : IEventMessage
        {
            _eventService.Unsubscribe(subscriberId);
        }

        /// <summary>
        /// Unsubscribe the specified handler using subscriber identifier.
        /// </summary>
        /// <param name="subscriberId">subscriber identifier</param>
        public void Unsubscribe(Guid subscriberId)
        {
            _eventService.Unsubscribe(subscriberId);
        }

        /// <summary>
        /// Dispose event service.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose event service resources.
        /// </summary>
        /// <param name="disposing">flag indicating disposing</param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _eventService.Dispose();
        }
    }
}
