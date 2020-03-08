using System;

namespace CafeLib.Core.Eventing
{
    internal abstract class EventSubscriber
    {
        private readonly object _action;

        public Guid Id { get; }

        protected bool IsAlive => true; 

        protected T GetTarget<T>() => (T) _action;

        public void Invoke<T>(T message)
        {
            if (!IsAlive) return;
            GetTarget<Action<T>>().Invoke(message);
        }

        protected EventSubscriber(object action)
        {
            _action = action;
            Id = Guid.NewGuid();
        }
    }

    internal class EventSubscriber<T> : EventSubscriber where T : IEventMessage
    {
        public EventSubscriber(Action<T> action)
            : base(action)
        {
        }
    }
}