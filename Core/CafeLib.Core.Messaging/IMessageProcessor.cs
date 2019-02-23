using CafeLib.Core.Queueing;

namespace CafeLib.Core.Messaging
{
    public interface IMessageProcessor : IQueueConsumer<IMessage>
    {
    }
}
