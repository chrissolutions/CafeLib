using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Eventing.Subscribers
{
    internal class ActionEventSubscriber<T> : EventSubscriber<T> where T : IEventMessage
    {
        public ActionEventSubscriber(Action<T> action)
            : base(action)
        {
        }

        public override Task Invoke(T message)
        {
            if (IsAlive)
            {
                var action = GetTarget<Action<T>>();
                action(message);
            }

            return Task.CompletedTask;
        }
    }
}