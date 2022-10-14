using System;
using System.Threading.Tasks;

namespace CafeLib.Core.Eventing.Subscribers
{
    internal class TaskEventSubscriber<T> : EventSubscriber<T> where T : IEventMessage
    {
        public TaskEventSubscriber(Func<T, Task> operation)
            : base(operation)
        {
        }

        public override Task Invoke(T message)
        {
            return !IsAlive ? Task.CompletedTask : GetTarget<Func<T, Task>>().Invoke(message);
        }
    }
}