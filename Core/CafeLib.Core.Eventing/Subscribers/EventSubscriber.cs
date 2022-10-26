using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Eventing.Subscribers
{
    internal abstract class EventSubscriber
    {
        private readonly object _operation;

        public Guid Id { get; }

        protected bool IsAlive => true;

        protected TOperation GetTarget<TOperation>() => (TOperation)_operation;

        public Task Invoke<TEventMessage>(TEventMessage message) where TEventMessage : IEventMessage 
            => ((IInvokeSubscriber<TEventMessage>)this).Invoke(message);

        protected EventSubscriber(object operation)
        {
            _operation = operation;
            Id = Guid.NewGuid();
        }
    }

    internal abstract class EventSubscriber<T> : EventSubscriber, IInvokeSubscriber<T> where T : IEventMessage
    {
        protected EventSubscriber(object operation)
            : base(operation)
        {
        }

        public abstract Task Invoke(T message);
    }
}