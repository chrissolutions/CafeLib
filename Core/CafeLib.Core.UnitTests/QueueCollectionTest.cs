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
    }
}