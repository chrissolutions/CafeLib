using CafeLib.Core.Queueing;

namespace CafeLib.Core.Messaging
{
    public class MessageQueue : QueueController<IMessage>
    {
        public MessageQueue(IQueueConsumer<IMessage> eventConsumer)
            : base(eventConsumer)
        {
        }
    }
}
