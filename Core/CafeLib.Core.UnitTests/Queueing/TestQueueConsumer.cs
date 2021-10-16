using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CafeLib.Core.Queueing;

namespace CafeLib.Core.UnitTests.Queueing
{
    public class TestQueueConsumer : QueueConsumer<TestQueueItem>
    {
        public Action<TestQueueItem> Notify { get; set; }

        public override Task Consume(TestQueueItem item)
        {
            Notify(item);
            Debug.WriteLine($"item id = {item.Id} item name = {item.Name}");
            return Task.CompletedTask;
        }
    }
}
