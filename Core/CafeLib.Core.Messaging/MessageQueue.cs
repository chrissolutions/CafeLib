using CafeLib.Core.Eventing;
using CafeLib.Core.Queueing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Messaging
{
    public class MessageQueue : QueueController<IEventMessage>
    {
        public MessageQueue(IQueueConsumer<IEventMessage> eventConsumer)
            : base(eventConsumer)
        {
        }
    }
}
