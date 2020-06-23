using CafeLib.Core.Eventing;
using CafeLib.Core.Queueing.Producer;

namespace CafeLib.Core.Messaging
{
    public interface IMessageProcessor : IQueueConsumer<IEventMessage>
    {
    }
}
