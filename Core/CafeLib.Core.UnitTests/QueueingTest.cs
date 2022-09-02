using System.Threading;
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
        private static readonly object Mutex = new();
        private const int Limit = 10;

        [Fact]
        public async Task ProducerConsumerTest()
        {
            var producer = new TestProducerQueue();
            var consumer1 = new TestQueueConsumer {Notify = Verify};
            var consumer2 = new TestQueueConsumer {Notify = Verify};

            _counter = 0;
            _consumersPerProducer = 2;

            producer.Add(consumer1);
            producer.Add(consumer2);
            
            await producer.Start();

            for (var x = 0; x < Limit; ++x)
            {
                producer.Produce(new TestQueueItem{Id = x, Name = $"Item{x}"});
            }

            await producer.Stop();
        }

        [Fact]
        public async Task PriorityProducerConsumerTest()
        {
            var producer = new TestPriorityProducerQueue();
            var consumer1 = new TestQueueConsumer { Notify = Verify };
            var consumer2 = new TestQueueConsumer { Notify = Verify };

            _counter = 0;
            _consumersPerProducer = 2;

            producer.Add(consumer1);
            producer.Add(consumer2);

            await producer.Start();

            for (var x = 0; x < Limit; ++x)
            {
                producer.Produce(new TestQueueItem { Id = x, Name = $"Item{x}" });
            }

            await producer.Stop();
        }

        private void Verify(TestQueueItem item)
        {
            lock (Mutex)
            {
                Assert.True(_counter <= Limit);
                Assert.NotNull(item);
                var id = _counter / _consumersPerProducer;
                Assert.Equal(id, item.Id);
                Assert.Equal($"Item{id}", item.Name);
                ++_counter;
            }
        }
    }
}