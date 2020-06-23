using CafeLib.Core.Eventing;
using CafeLib.Core.Queueing.Producer;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Messaging
{
    public class MessageQueue<T> : QueueProducer<T> where T : IEventMessage
    {
        public MessageQueue(IQueueConsumer<T> eventConsumer)
            : base(eventConsumer)
        {
        }
    }
}
