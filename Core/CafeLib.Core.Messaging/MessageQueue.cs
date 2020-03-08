using CafeLib.Core.Eventing;
using CafeLib.Core.Queueing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Messaging
{
    public class MessageQueue : QueueProducer<IEventMessage>
    {
        public MessageQueue(IQueueConsumer<IEventMessage> eventConsumer)
            : base(eventConsumer)
        {
        }
    }
}
