using System.Threading.Tasks;
using CafeLib.Core.Queueing;
using CafeLib.Core.UnitTests.Queueing;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class QueueingTest
    {
        private int _counter;
        private int _consumersPerProducer;

        [Fact]
        public async Task ProducerConsumerTest()
        {
            var producer = new TestProducerQueue();
            var consumer1 = new TestQueueConsumer {Notify = Verify};
            var consumer2 = new TestQueueConsumer {Notify = Verify};

            _counter = 0;
            _consumersPerProducer = 2;

            QueueBroker.Current.Subscribe(consumer1, producer);
            QueueBroker.Current.Subscribe(consumer2, producer);


            await producer.Start();

            for (var x = 0; x < 10; ++x)
            {
                producer.Produce(new TestQueueItem{Id = x, Name = $"Item{x}"});
            }

            await producer.Stop();
        }

        private void Verify(TestQueueItem item)
        {
            Assert.NotNull(item);
            var id = _counter / _consumersPerProducer;
            Assert.Equal(id, item.Id);
            Assert.Equal($"Item{id}", item.Name);
            ++_counter;
        }
    }
}