using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Collections.Queues;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class QueueCollectionTest
    {
        [Fact]
        public void PriorityQueueDequeTest()
        {
            var priorityQueue = new PriorityQueue<string>();

            priorityQueue.Enqueue("This is a message with normal priority");
            priorityQueue.Enqueue("This is a message two with normal priority");

            Assert.Equal("This is a message with normal priority", priorityQueue.Dequeue());
            Assert.Equal("This is a message two with normal priority", priorityQueue.Dequeue());
        }

        [Fact]
        public void PriorityQueuePriorityDequeTest()
        {
            var priorityQueue = new PriorityQueue<string>();

            priorityQueue.Enqueue("This is a message A with normal priority");
            priorityQueue.Enqueue("This is a message B with normal priority");
            priorityQueue.Enqueue("This is a message C with normal priority");
            priorityQueue.Enqueue("This is a message D with normal priority");
            priorityQueue.Enqueue("This is a message with higher priority", 5);

            Assert.Equal("This is a message with higher priority", priorityQueue.Dequeue());
            Assert.Equal("This is a message A with normal priority", priorityQueue.Dequeue());
            Assert.Equal("This is a message B with normal priority", priorityQueue.Dequeue());
            Assert.Equal("This is a message C with normal priority", priorityQueue.Dequeue());
            Assert.Equal("This is a message D with normal priority", priorityQueue.Dequeue());
        }

        [Fact]
        public void ReaderWriterQueueTest()
        {
            var producer = new ReaderWriterQueue<string>();
            var resetEvent = new ManualResetEventSlim(false);

            var _ = Task.Factory.StartNew(() =>
            {
                var item = producer.Dequeue();
                Assert.Equal("This is a message", item);
                // ReSharper disable once AccessToDisposedClosure
                resetEvent.Set();
            });

            producer.Enqueue("This is a message");
            resetEvent.Wait();
            resetEvent.Dispose();
        }

        [Fact]
        public void ReaderWriterPriorityQueueTest()
        {
            var producer = new ReaderWriterPriorityQueue<string>();
            var resetEvent = new ManualResetEventSlim(false);

            var _ = Task.Factory.StartNew(() =>
            {
                var item = producer.Dequeue();
                Assert.Equal("This is a higher message", item);
                // ReSharper disable once AccessToDisposedClosure
                resetEvent.Set();
            });

            producer.Enqueue("This is a message");
            producer.Enqueue("This is a higher message", 2);
            resetEvent.Wait();
            resetEvent.Dispose();
        }
    }
}