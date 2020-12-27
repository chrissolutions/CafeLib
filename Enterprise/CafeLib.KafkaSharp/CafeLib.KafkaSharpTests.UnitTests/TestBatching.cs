using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CafeLib.KafkaSharp.Batching;
using CafeLib.KafkaSharp.Cluster;
using Xunit;

namespace CafeLib.KafkaSharpTests.UnitTests
{
    public class TestBatching
    {
        [Fact]
        public void TestAccumulatorByTopicCountReached()
        {
            using var accumulator = new AccumulatorByTopic<Tuple<string, int>>(t => t.Item1, 5,
                TimeSpan.FromMilliseconds(1000000));
            IBatchByTopic<Tuple<string, int>> batch = null;
            accumulator.NewBatch += b => batch = b;
            accumulator.Add(Tuple.Create("a", 1));
            accumulator.Add(Tuple.Create("a", 2));
            accumulator.Add(Tuple.Create("b", 8));
            accumulator.Add(Tuple.Create("a", 3));
            accumulator.Add(Tuple.Create("c", 1));
            accumulator.Add(Tuple.Create("a", 1));

            Assert.Equal(5, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Equal(1, batch.Count(g => g.Key == "b"));
            Assert.Equal(1, batch.Count(g => g.Key == "c"));
            Assert.Equal(new[] {1, 2, 3}, batch.First(g => g.Key == "a").Select(t => t.Item2));
            Assert.Equal(new[] {8}, batch.First(g => g.Key == "b").Select(t => t.Item2));
            Assert.Equal(new[] {1}, batch.First(g => g.Key == "c").Select(t => t.Item2));

            accumulator.Add(Tuple.Create("a", 1));
            accumulator.Add(Tuple.Create("a", 1));
            accumulator.Add(Tuple.Create("a", 1));
            accumulator.Add(Tuple.Create("a", 1));

            Assert.Equal(5, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Single(batch);
            Assert.Equal(5, batch.First().Count());
            Assert.Equal(5, batch.First().Count(t => t.Item2 == 1));
            batch.Dispose();
        }

        [Fact]
        public void TestAccumulatorByTopicTimeElapsed()
        {
            using var accumulator = new AccumulatorByTopic<Tuple<string, int>>(t => t.Item1, 5, TimeSpan.FromMilliseconds(15));
            var latch = new ManualResetEventSlim();
            IBatchByTopic<Tuple<string, int>> batch = null;
            accumulator.NewBatch += b =>
            {
                latch.Set();
                batch = b;
            };
            Assert.True(accumulator.Add(Tuple.Create("a", 1)));
            Assert.True(accumulator.Add(Tuple.Create("b", 2)));
            Assert.True(accumulator.Add(Tuple.Create("c", 3)));

            latch.Wait();

            Assert.Equal(3, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Equal(1, batch.Count(g => g.Key == "b"));
            Assert.Equal(1, batch.Count(g => g.Key == "c"));
            Assert.Equal(new[] {1}, batch.First(g => g.Key == "a").Select(t => t.Item2));
            Assert.Equal(new[] {2}, batch.First(g => g.Key == "b").Select(t => t.Item2));
            Assert.Equal(new[] {3}, batch.First(g => g.Key == "c").Select(t => t.Item2));
            batch.Dispose();
        }

        [Fact]
        public void TestAccumulatorByTopicByPartitionCountReached()
        {
            using var accumulator = new AccumulatorByTopicByPartition<Tuple<string, int, int>>(t => t.Item1, t => t.Item2,
                5,
                TimeSpan.FromMilliseconds(1000000));
            IBatchByTopicByPartition<Tuple<string, int, int>> batch = null;
            accumulator.NewBatch += b => batch = b;
            accumulator.Add(Tuple.Create("a", 1, 1));
            accumulator.Add(Tuple.Create("a", 1, 2));
            accumulator.Add(Tuple.Create("b", 1, 8));
            accumulator.Add(Tuple.Create("a", 2, 3));
            accumulator.Add(Tuple.Create("c", 1, 1));
            accumulator.Add(Tuple.Create("a", 1, 1));

            Assert.Equal(5, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Equal(1, batch.Count(g => g.Key == "b"));
            Assert.Equal(1, batch.Count(g => g.Key == "c"));
            Assert.Equal(new[] {1, 2}, batch.First(g => g.Key == "a").Where(g => g.Key == 1).SelectMany(g => g).Select(t => t.Item3));
            Assert.Equal(new[] {1, 2, 3}, batch.First(g => g.Key == "a").SelectMany(g => g).Select(t => t.Item3));
            Assert.Equal(new[] {8}, batch.First(g => g.Key == "b").SelectMany(g => g.Select(t => t.Item3)));
            Assert.Equal(new[] {1}, batch.First(g => g.Key == "c").SelectMany(g => g.Select(t => t.Item3)));

            accumulator.Add(Tuple.Create("a", 1, 1));
            accumulator.Add(Tuple.Create("a", 1, 1));
            accumulator.Add(Tuple.Create("a", 2, 1));
            accumulator.Add(Tuple.Create("a", 2, 1));

            Assert.Equal(5, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Single(batch);
            Assert.Equal(2, batch.First().Count());
            Assert.Equal(3, batch.First().Where(g => g.Key == 1).SelectMany(g => g).Count(t => t.Item3 == 1));
            Assert.Equal( 2, batch.First().Where(g => g.Key == 2).SelectMany(g => g).Count(t => t.Item3 == 1));
            Assert.Equal(5, batch.First().SelectMany(g => g).Count(t => t.Item3 == 1));
            batch.Dispose();
        }

        [Fact]
        public void TestAccumulatorByTopicByPartitionTimeElapsed()
        {
            using var accumulator = new AccumulatorByTopicByPartition<Tuple<string, int, int>>(t => t.Item1, t => t.Item2,
                5,
                TimeSpan.FromMilliseconds(150));
            var latch = new ManualResetEventSlim();
            IBatchByTopicByPartition<Tuple<string, int, int>> batch = null;
            accumulator.NewBatch += b =>
            {
                batch = b;
                latch.Set();
            };
            Assert.True(accumulator.Add(Tuple.Create("a", 1, 1)));
            Assert.True(accumulator.Add(Tuple.Create("b", 1, 2)));
            Assert.True(accumulator.Add(Tuple.Create("c", 1, 3)));

            latch.Wait();

            Assert.Equal(3, batch.Count);
            Assert.Equal(1, batch.Count(g => g.Key == "a"));
            Assert.Equal(1, batch.Count(g => g.Key == "b"));
            Assert.Equal(1, batch.Count(g => g.Key == "c"));
            Assert.Equal(new[] {1}, batch.First(g => g.Key == "a").SelectMany(g => g).Select(t => t.Item3));
            Assert.Equal(new[] {2}, batch.First(g => g.Key == "b").SelectMany(g => g).Select(t => t.Item3));
            Assert.Equal(new[] {3}, batch.First(g => g.Key == "c").SelectMany(g => g).Select(t => t.Item3));
            batch.Dispose();
        }

        [Theory]
        [InlineData(5, 100000000)]
        [InlineData(500000, 100)]
        public void TestAccumulatorByNodeByTopic(int batchSize, int time)
        {
            INode n1 = new NodeMock("n1");
            INode n2 = new NodeMock("n2");
            var accumulator = new AccumulatorByNodeByTopic<Tuple<string, int>>(t => t.Item1, batchSize, TimeSpan.FromMilliseconds(time));

            var count = new CountdownEvent(2);
            var d = new Dictionary<INode, IBatchByTopic<Tuple<string, int>>>();
            accumulator.NewBatch += (n, b) =>
            {
                d[n] = b;
                count.Signal();
            };

            accumulator.Add(Tuple.Create(n1, Tuple.Create("1", 1)));
            accumulator.Add(Tuple.Create(n1, Tuple.Create("1", 2)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 1)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 2)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 3)));

            count.Wait();

            Assert.Equal(2, d[n1].Count);
            Assert.Equal(3, d[n2].Count);
            d[n1].Dispose();
            d[n2].Dispose();
        }

        [Theory]
        [InlineData(5, 100000000)]
        [InlineData(500000, 100)]
        public void TestAccumulatorByNodeByTopicByPartition(int batchSize, int time)
        {
            INode n1 = new NodeMock("n1");
            INode n2 = new NodeMock("n2");
            var accumulator = new AccumulatorByNodeByTopicByPartition<Tuple<string, int, int>>(t => t.Item1, t => t.Item2, batchSize, TimeSpan.FromMilliseconds(time));

            var count = new CountdownEvent(2);
            var d = new Dictionary<INode, IBatchByTopicByPartition<Tuple<string, int, int>>>();
            accumulator.NewBatch += (n, b) =>
            {
                d[n] = b;
                count.Signal();
            };

            accumulator.Add(Tuple.Create(n1, Tuple.Create("1", 1, 1)));
            accumulator.Add(Tuple.Create(n1, Tuple.Create("1", 1, 2)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 1, 1)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 2, 2)));
            accumulator.Add(Tuple.Create(n2, Tuple.Create("2", 2, 3)));

            count.Wait();

            Assert.Equal(2, d[n1].Count);
            Assert.Equal(3, d[n2].Count);
            d[n1].Dispose();
            d[n2].Dispose();
        }

        [Fact]
        public void TestDisposeAccumulator()
        {
            var accumulator = new AccumulatorByTopic<Tuple<string, int>>(t => t.Item1, 5,
                TimeSpan.FromMilliseconds(1000000));
            IBatchByTopic<Tuple<string, int>> batch = null;
            accumulator.NewBatch += b => batch = b;
            Assert.True(accumulator.Add(Tuple.Create("a", 1)));
            Assert.True(accumulator.Add(Tuple.Create("a", 2)));
            Assert.True(accumulator.Add(Tuple.Create("a", 3)));
            Assert.Null(batch);

            accumulator.Dispose();

            Assert.NotNull(batch);
            Assert.Equal( 3, batch.Count);
            Assert.False(accumulator.Add(Tuple.Create("a", 1)));
        }
    }
}