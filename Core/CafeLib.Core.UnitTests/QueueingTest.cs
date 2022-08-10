using System.Threading.Tasks;
using CafeLib.Core.Collections;
using CafeLib.Core.UnitTests.Queueing;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class QueueingTest
    {
        private int _counter;
        private int _consumersPerProducer;
        private int _id;
        private static readonly object Mutex = new();

        [Fact]
        public async Task ProducerConsumerTest()
        {
            var producer = new TestProducerQueue();
            var consumer1 = new TestQueueConsumer {Notify = Verify};
            var consumer2 = new TestQueueConsumer {Notify = Verify};

            _counter = 0;
            _id = 0;
            _consumersPerProducer = 2;
            const int limit = 10;

            producer.Add(consumer1);
            producer.Add(consumer2);
            
            await producer.Start();

            for (var x = 0; x < limit; ++x)
            {
                producer.Produce(new TestQueueItem{Id = x, Name = $"Item{x}"});
            }

            await producer.Stop();

            Assert.Equal(limit, _id + 1);
        }

        [Fact]
        public async Task PriorityProducerConsumerTest()
        {
            var producer = new TestPriorityProducerQueue();
            var consumer1 = new TestQueueConsumer { Notify = Verify };
            var consumer2 = new TestQueueConsumer { Notify = Verify };

            _counter = 0;
            _id = 0;
            _consumersPerProducer = 2;
            const int limit = 10;

            producer.Add(consumer1);
            producer.Add(consumer2);

            await producer.Start();

            for (var x = 0; x < limit; ++x)
            {
                producer.Produce(new TestQueueItem { Id = x, Name = $"Item{x}" });
            }

            await producer.Stop();

            Assert.Equal(limit, _id + 1);
        }

        private void Verify(TestQueueItem item)
        {
            lock (Mutex)
            {
                Assert.NotNull(item);
                _id = _counter / _consumersPerProducer;
                Assert.Equal(_id, item.Id);
                Assert.Equal($"Item{_id}", item.Name);
                ++_counter;
            }
        }
    }
}