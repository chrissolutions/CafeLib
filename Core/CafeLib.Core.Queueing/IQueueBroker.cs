// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Queueing
{
    public interface IQueueBroker
    {
        /// <summary>
        /// Register producer to broker.
        /// </summary>
        /// <param name="producer">producer</param>
        void Register(IQueueProducer producer);

        /// <summary>
        /// Unregister producer from broker.
        /// </summary>
        /// <param name="producer"></param>
        void Unregister(IQueueProducer producer);

        /// <summary>
        /// Subscribe consumer to a producer..
        /// </summary>
        /// <param name="consumer">consumer</param>
        /// <param name="producer">producer</param>
        void Subscribe(IQueueConsumer consumer, IQueueProducer producer);

        /// <summary>
        /// Unsubscribe consumer from producer.
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="producer"></param>
        void Unsubscribe(IQueueConsumer consumer, IQueueProducer producer);
    }
}
