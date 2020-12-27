using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CafeLib.KafkaSharp;
using CafeLib.KafkaSharp.Cluster;
using CafeLib.KafkaSharp.Common;
using CafeLib.KafkaSharp.Protocol;
using CafeLib.KafkaSharp.Routing;
using Moq;
using Xunit;

namespace CafeLib.KafkaSharpTests.UnitTests
{
    public class TestClient : IDisposable
    {
        private ClusterClient _client;
        private Mock<IProduceRouter> _producer;
        private Mock<IConsumeRouter> _consumer;
        private Mock<INode> _node;

        private void Init(Configuration configuration)
        {
            _node = new Mock<INode>();
            var brokerMeta = new BrokerMeta {Host = "brokerHost"};
            _node.Setup(n => n.FetchMetadata())
                .Returns(Task.FromResult(new MetadataResponse
                {
                    BrokersMeta = new[] {brokerMeta}, TopicsMeta = new TopicMeta[0]
                }));
            _node.Setup(n => n.Equals(It.IsAny<object>())).Returns(true);
            _node.Setup(n => n.Equals(It.IsAny<INode>())).Returns(true);
            _node.Setup(n => n.GetHashCode()).Returns(0);
            _producer = new Mock<IProduceRouter>();
            _consumer = new Mock<IConsumeRouter>();
            var logger = new Mock<ILogger>();
            _client = new ClusterClient(configuration, logger.Object,
                new Cluster(configuration, logger.Object, (h, p) => _node.Object, () => _producer.Object,
                    () => _consumer.Object));
        }

        public TestClient()
        {
            var configuration = new Configuration { Seeds = "localhost:1", TaskScheduler = new CurrentThreadTaskScheduler() };
            Init(configuration);
        }

        private const string Topic = "topic";
        private const string Key = "Key";
        private const string Value = "Value";

        private readonly byte[] _keyB = Encoding.UTF8.GetBytes(Key);
        private readonly byte[] _valueB = Encoding.UTF8.GetBytes(Value);

        private static bool AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> compared)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (compared == null) throw new ArgumentNullException(nameof(compared));
            Assert.Equal(expected, compared);
            return true;
        }

        [Fact]
        public void TestProduceValue()
        {
            Assert.True(_client.Produce(Topic, _valueB));
            _producer.Verify(p => p.Route(It.IsAny<string>(), It.IsAny<Message>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once());
            _producer.Verify(
                p =>
                    p.Route(
                        Topic,
                        It.Is<Message>(m => m.Key == null && AreEqual(_valueB, m.Value as byte[])),
                        Partitions.Any,
                        It.Is<DateTime>(d => d != default)));
        }

        [Fact]
        public void TestProduceKeyValue()
        {
            Assert.True(_client.Produce(Topic, _keyB, _valueB));
            _producer.Verify(p => p.Route(It.IsAny<string>(), It.IsAny<Message>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once());
            _producer.Verify(
                p =>
                    p.Route(
                        Topic,
                        It.Is<Message>(m => AreEqual(_keyB, m.Key as byte[]) && AreEqual(_valueB, m.Value as byte[])),
                        Partitions.Any,
                        It.Is<DateTime>(d => d != default)));
        }

        [Fact]
        public void TestProduceKeyValuePartition()
        {
            Assert.True(_client.Produce(Topic, _keyB, _valueB, 28));
            _producer.Verify(p => p.Route(It.IsAny<string>(), It.IsAny<Message>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once());
            _producer.Verify(
                p =>
                    p.Route(
                        Topic,
                        It.Is<Message>(m => AreEqual(_keyB, m.Key as byte[]) && AreEqual(_valueB, m.Value as byte[])),
                        28,
                        It.Is<DateTime>(d => d != default)));
        }

        private void VerifyConsume(string topic, int partition, long offset)
        {
            _consumer.Verify(c => c.StartConsume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
            _consumer.Verify(c => c.StartConsume(It.Is<string>(s => s == topic), It.Is<int>(p => p == partition), It.Is<long>(o => o == offset)));
        }

        [Fact]
        public void TestConsume()
        {
            const int p = 1235;
            const long o = 76158134069;
            _client.Consume(Topic, p, o);

            VerifyConsume(Topic, p, o);
        }

        [Fact]
        public void TestConsumeFromLatest()
        {
            const int p = 1235;
            _client.ConsumeFromLatest(Topic, p);

            VerifyConsume(Topic, p, Offsets.Latest);
        }

        [Fact]
        public void TestConsumeAllFromLatest()
        {
            _client.ConsumeFromLatest(Topic);

            VerifyConsume(Topic, Partitions.All, Offsets.Latest);
        }

        [Fact]
        public void TestConsumeFromEarliest()
        {
            const int p = 1235;
            _client.ConsumeFromEarliest(Topic, p);

            VerifyConsume(Topic, p, Offsets.Earliest);
        }

        [Fact]
        public void TestConsumeAllFromEarliest()
        {
            _client.ConsumeFromEarliest(Topic);

            VerifyConsume(Topic, Partitions.All, Offsets.Earliest);
        }

        private void VerifyStopConsume(string topic, int partition, long offset)
        {
            _consumer.Verify(c => c.StopConsume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
            _consumer.Verify(c => c.StopConsume(It.Is<string>(s => s == topic), It.Is<int>(p => p == partition), It.Is<long>(o => o == offset)));
        }

        [Fact]
        public void TestStopConsumePartition()
        {
            const int p = 1235;
            const long o = 76158134069;
            _client.StopConsume(Topic, p, o);

            VerifyStopConsume(Topic, p, o);
        }

        [Fact]
        public void TestStopConsumePartitionNow()
        {
            const int p = 1235;
            _client.StopConsume(Topic, p);

            VerifyStopConsume(Topic, p, Offsets.Now);
        }

        [Fact]
        public void TestStopConsumeAllNow()
        {
            _client.StopConsume(Topic);

            VerifyStopConsume(Topic, Partitions.All, Offsets.Now);
        }

        [Fact]
        public void TestConsumeErrors()
        {
            Assert.Throws<ArgumentException>(() => _client.Consume(Topic, -8, 213423));
            Assert.Throws<ArgumentException>(() => _client.Consume(Topic, 3, -213423));
            Assert.Throws<ArgumentException>(() => _client.ConsumeFromLatest(Topic, -8));
            Assert.Throws<ArgumentException>(() => _client.ConsumeFromEarliest(Topic, -8));
            Assert.Throws<ArgumentException>(() => _client.StopConsume(Topic, -8, 215));
            Assert.Throws<ArgumentException>(() => _client.StopConsume(Topic, 1, -215));
            Assert.Throws<ArgumentException>(() => _client.StopConsume(Topic, -8));
        }

        [Fact]
        public void TestStats()
        {
            Assert.NotNull(_client.Statistics);
            Assert.NotNull(_client.Statistics.ToString());
        }

        private static void AssertRecords(RawKafkaRecord expected, RawKafkaRecord record)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (record == null) throw new ArgumentNullException(nameof(record));
            Assert.NotNull(record);
            Assert.Equal(expected.Topic, record.Topic);
            Assert.Equal(expected.Key as byte[], record.Key as byte[]);
            Assert.Equal(expected.Value as byte[], record.Value as byte[]);
            Assert.Equal(expected.Partition, record.Partition);
            Assert.Equal(expected.Offset, record.Offset);
        }

        [Fact]
        public void TestMessageReceived()
        {
            RawKafkaRecord record = null;
            _client.MessageReceived += kr => record = kr;
            var expected = new RawKafkaRecord {Topic = Topic, Key = _keyB, Value = _valueB, Partition = 1, Offset = 123};
            _consumer.Raise(c => c.MessageReceived += null, expected);
            AssertRecords(expected, record);
        }

        [Fact]
        public void TestMessageReceivedObservable()
        {
            RawKafkaRecord record = null;
            _client.Messages.Subscribe(kr => record = kr);
            var expected = new RawKafkaRecord { Topic = Topic, Key = _keyB, Value = _valueB, Partition = 1, Offset = 123 };
            _consumer.Raise(c => c.MessageReceived += null, expected);
            AssertRecords(expected, record);
        }

        [Fact]
        public void TestMessageExpired()
        {
            RawKafkaRecord record = null;
            _client.MessageExpired += kr => record = kr;
            _producer.Raise(c => c.MessageExpired += null, Topic, new Message {Key = _keyB, Value = _valueB});
            AssertRecords(
                new RawKafkaRecord {Topic = Topic, Key = _keyB, Value = _valueB, Partition = Partitions.None, Offset = 0},
                record);
        }

        [Fact]
        public void TestMessageExpiredObservable()
        {
            RawKafkaRecord record = null;
            _client.ExpiredMessages.Subscribe(kr => record = kr);
            _producer.Raise(c => c.MessageExpired += null, Topic, new Message {Key = _keyB, Value = _valueB});
            AssertRecords(
                new RawKafkaRecord {Topic = Topic, Key = _keyB, Value = _valueB, Partition = Partitions.None, Offset = 0},
                record);
        }

        [Fact]
        public void TestMessageDiscarded()
        {
            RawKafkaRecord record = null;
            _client.MessageDiscarded += kr => record = kr;
            _producer.Raise(c => c.MessageDiscarded += null, Topic, new Message { Key = _keyB, Value = _valueB });
            AssertRecords(
                new RawKafkaRecord { Topic = Topic, Key = _keyB, Value = _valueB, Partition = Partitions.None, Offset = 0 },
                record);
        }

        [Fact]
        public void TestMessageDiscardedObservable()
        {
            _client.DiscardedMessages.Subscribe(kr =>
            {
                AssertRecords(
                    new RawKafkaRecord { Topic = Topic, Key = _keyB, Value = _valueB, Partition = Partitions.None, Offset = 0 },
                    kr);
            });

            _producer.Raise(c => c.MessageDiscarded += null, Topic, new Message { Key = _keyB, Value = _valueB });
        }

        [Fact]
        public void TestProduceAcknowledged()
        {
            int count = 0;
            string topic = "";
            _client.ProduceAcknowledged += (t, n) =>
            {
                topic = t;
                count = n;
            };
            _producer.Raise(c => c.MessagesAcknowledged += null, Topic, 28);
            Assert.Equal(28, count);
            Assert.Equal(Topic, topic);
        }

        [Fact]
        public void TestProducer()
        {
            var client = new Mock<IClusterClient>();
            client.SetupGet(c => c.DiscardedMessages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageDiscarded += a,
                    a => client.Object.MessageDiscarded -= a));
            client.SetupGet(c => c.ExpiredMessages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageExpired += a,
                    a => client.Object.MessageExpired -= a));

            // Bad arguments
            Assert.Throws<ArgumentException>(() => new KafkaProducer<string, string>(null, client.Object));
            Assert.Throws<ArgumentException>(() => new KafkaProducer<string, string>("", client.Object));
            Assert.Throws<ArgumentNullException>(() => new KafkaProducer<string, string>("toto", null));

            using (var producer = new KafkaProducer<string, string>("topic", client.Object))
            {
                // Double new on same topic/TKey/TValue
                Assert.Throws<ArgumentException>(() => new KafkaProducer<string, string>("topic", client.Object));

                // Produce are forwarded to underlying cluster client
                producer.Produce("data");
                producer.Produce("key", "data");
                producer.Produce("key", "data", 42);

                client.Verify(c => c.Produce(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(),
                    It.IsAny<ICollection<KafkaRecordHeader>>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Exactly(3));
                client.Verify(c => c.Produce("topic", null, "data", null, Partitions.Any, It.IsAny<DateTime>()), Times.Once());
                client.Verify(c => c.Produce("topic", "key", "data", null, Partitions.Any, It.IsAny<DateTime>()), Times.Once());
                client.Verify(c => c.Produce("topic", "key", "data", null, 42, It.IsAny<DateTime>()), Times.Once());

                // Discarded/Expired messages are correctly transmitted
                bool discardedThroughEvent = false;
                bool observedDiscard = false;
                bool expiredThroughEvent = false;
                bool observedExpired = false;

                Action<KafkaRecord<string, string>> checkRecord = kr =>
                {
                    Assert.Equal("topic", kr.Topic);
                    Assert.Equal("key", kr.Key);
                    Assert.Equal("data", kr.Value);
                    Assert.Equal(Partitions.None, kr.Partition);
                    Assert.Equal(0, kr.Offset);
                };

                producer.MessageDiscarded += kr =>
                {
                    checkRecord(kr);
                    discardedThroughEvent = true;
                };

                producer.MessageExpired += kr =>
                {
                    checkRecord(kr);
                    expiredThroughEvent = true;
                };

                producer.DiscardedMessages.Subscribe(kr =>
                {
                    checkRecord(kr);
                    observedDiscard = true;
                });

                producer.ExpiredMessages.Subscribe(kr =>
                {
                    checkRecord(kr);
                    observedExpired = true;
                });

                var record = new RawKafkaRecord
                {
                    Topic = "topic",
                    Key = "key",
                    Value = "data",
                    Partition = Partitions.None
                };
                client.Raise(c => c.MessageDiscarded += null, record);
                client.Raise(c => c.MessageExpired += null, record);

                Assert.True(discardedThroughEvent);
                Assert.True(expiredThroughEvent);
                Assert.True(observedDiscard);
                Assert.True(observedExpired);
            }

            // Dispose: can register another producer with same Topic/TKey/TValue once
            // the previous one has been disposed.
            client = new Mock<IClusterClient>();
            client.SetupGet(c => c.DiscardedMessages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageDiscarded += a,
                    a => client.Object.MessageDiscarded -= a));
            client.SetupGet(c => c.ExpiredMessages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageExpired += a,
                    a => client.Object.MessageExpired -= a));
            var producer2 = new KafkaProducer<string, string>("topic", client.Object);

            // Dispose: observable are completed and events no longer subscribed
            bool discardedCompleted = false;
            bool expiredCompleted = false;
            bool discardedEvent = false;
            bool expiredEvent = false;
            producer2.DiscardedMessages.Subscribe(kr => { }, () => discardedCompleted = true);
            producer2.ExpiredMessages.Subscribe(kr => { }, () => expiredCompleted = true);
            producer2.MessageDiscarded += _ => discardedEvent = true;
            producer2.MessageExpired += _ => expiredEvent = true;
            producer2.Dispose();

            var record2 = new RawKafkaRecord
            {
                Topic = "topic",
                Key = "key",
                Value = "data",
                Partition = Partitions.None
            };
            client.Raise(c => c.MessageDiscarded += null, record2);
            client.Raise(c => c.MessageExpired += null, record2);

            Assert.True(discardedCompleted);
            Assert.True(expiredCompleted);
            Assert.False(discardedEvent);
            Assert.False(expiredEvent);

            // Dispose: Produce do no longer work
            producer2.Produce("data");
            client.Verify(c => c.Produce(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), It.IsAny<int>()), Times.Never());

            // Dispose: can dispose the same producer multiple times with no effect
            producer2.Dispose();
        }

        [Fact]
        public void TestConsumer()
        {
            var client = new Mock<IClusterClient>();
            client.SetupGet(c => c.Messages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageReceived += a,
                    a => client.Object.MessageReceived -= a));

            // Bad arguments
            Assert.Throws<ArgumentException>(() => new KafkaProducer<string, string>(null, client.Object));
            Assert.Throws<ArgumentException>(() => new KafkaProducer<string, string>("", client.Object));
            Assert.Throws<ArgumentNullException>(() => new KafkaProducer<string, string>("toto", null));

            using (var consumer = new KafkaConsumer<string, string>("topic", client.Object))
            {
                // Double new on same topic/TKey/TValue
                Assert.Throws<ArgumentException>(() => new KafkaConsumer<string, string>("topic", client.Object));

                // Consume / Stop
                consumer.Consume(2, 42);
                consumer.ConsumeFromLatest();
                consumer.ConsumeFromLatest(2);
                consumer.ConsumeFromEarliest();
                consumer.ConsumeFromEarliest(2);
                consumer.StopConsume();
                consumer.StopConsume(2);
                consumer.StopConsume(2, 42);

                client.Verify(c => c.Consume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
                client.Verify(c => c.StopConsume("topic", 2, 42));
                client.Verify(c => c.ConsumeFromLatest(It.IsAny<string>()), Times.Once());
                client.Verify(c => c.ConsumeFromLatest("topic"));
                client.Verify(c => c.ConsumeFromLatest(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
                client.Verify(c => c.ConsumeFromLatest("topic", 2));
                client.Verify(c => c.ConsumeFromEarliest(It.IsAny<string>()), Times.Once());
                client.Verify(c => c.ConsumeFromEarliest("topic"));
                client.Verify(c => c.ConsumeFromEarliest(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
                client.Verify(c => c.ConsumeFromEarliest("topic", 2));

                client.Verify(c => c.StopConsume(It.IsAny<string>()), Times.Once());
                client.Verify(c => c.StopConsume("topic"));
                client.Verify(c => c.StopConsume(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
                client.Verify(c => c.StopConsume("topic", 2));
                client.Verify(c => c.StopConsume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
                client.Verify(c => c.StopConsume("topic", 2, 42));

                bool messageObserved = false;
                bool messageEvent = false;
                KafkaRecord<string, string> received = default(KafkaRecord<string, string>);

                consumer.MessageReceived += kr =>
                {
                    received = kr;
                    messageEvent = true;
                };
                consumer.Messages.Subscribe(kr =>
                {
                    messageObserved = true;
                });

                var record = new RawKafkaRecord
                {
                    Topic = "topic",
                    Key = "key",
                    Value = "data",
                    Partition = 2,
                    Offset = 42
                };

                client.Raise(c => c.MessageReceived += null, record);

                Assert.True(messageEvent);
                Assert.True(messageObserved);
                Assert.Equal("topic", received.Topic);
                Assert.Equal("key", received.Key);
                Assert.Equal("data", received.Value);
                Assert.Equal(2, received.Partition);
                Assert.Equal(42, received.Offset);

                record.Key = null;
                messageObserved = false;
                messageEvent = false;
                received = default(KafkaRecord<string, string>);
                client.Raise(c => c.MessageReceived += null, record);

                Assert.True(messageEvent);
                Assert.True(messageObserved);
                Assert.True(messageEvent);
                Assert.True(messageObserved);
                Assert.Equal("topic", received.Topic);
                Assert.Null(received.Key);
                Assert.Equal("data", received.Value);
                Assert.Equal(2, received.Partition);
                Assert.Equal(42, received.Offset);
            }

            // Dispose: can register another producer with same Topic/TKey/TValue once
            // the previous one has been disposed.
            client = new Mock<IClusterClient>();
            client.SetupGet(c => c.Messages)
                .Returns(Observable.FromEvent<RawKafkaRecord>(a => client.Object.MessageReceived += a,
                    a => client.Object.MessageReceived -= a));
            var consumer2 = new KafkaConsumer<string, string>("topic", client.Object);

            // Dispose: observable are completed and events no longer subscribed
            bool messageCompleted = false;
            bool messageEvent2 = false;
            consumer2.Messages.Subscribe(kr => { }, () => messageCompleted = true);
            consumer2.MessageReceived += _ => messageEvent2 = true;
            consumer2.Dispose();

            client.Verify(c => c.StopConsume(It.IsAny<string>()), Times.Once()); // Dispose stops all
            client.Verify(c => c.StopConsume("topic"), Times.Once());

            var record2 = new RawKafkaRecord
            {
                Topic = "topic",
                Key = "key",
                Value = "data",
                Partition = 2,
                Offset = 42
            };
            client.Raise(c => c.MessageReceived += null, record2);

            Assert.True(messageCompleted);
            Assert.False(messageEvent2);

            // Consume / Stop no longer work
            consumer2.Consume(2, 42);
            consumer2.ConsumeFromLatest();
            consumer2.ConsumeFromLatest(2);
            consumer2.ConsumeFromEarliest();
            consumer2.ConsumeFromEarliest(2);
            consumer2.StopConsume();
            consumer2.StopConsume(2);
            consumer2.StopConsume(2, 42);

            client.Verify(c => c.Consume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Never());
            client.Verify(c => c.ConsumeFromLatest(It.IsAny<string>()), Times.Never());
            client.Verify(c => c.ConsumeFromLatest(It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            client.Verify(c => c.ConsumeFromEarliest(It.IsAny<string>()), Times.Never());
            client.Verify(c => c.ConsumeFromEarliest(It.IsAny<string>(), It.IsAny<int>()), Times.Never());

            client.Verify(c => c.StopConsume(It.IsAny<string>()), Times.Once());
            client.Verify(c => c.StopConsume(It.IsAny<string>(), It.IsAny<int>()), Times.Never());
            client.Verify(c => c.StopConsume(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Never());

            // Dispose: can dispose the same consumer multiple times with no effect
            consumer2.Dispose();
        }

        [Fact]
        public void TestGetAllPartitionsForTopic()
        {
            _client.GetPartitionforTopicAsync("toto");
            _node.Verify(n => n.FetchMetadata(new [] { "toto"}));
        }

        [Fact]
        public void TestOverflowDiscard()
        {
            var configuration = new Configuration
            {
                Seeds = "localhost:1",
                TaskScheduler = new CurrentThreadTaskScheduler(),
                OverflowStrategy = OverflowStrategy.Discard,
                MaxBufferedMessages = 2
            };
            Init(configuration);
            Assert.True(_client.Produce(Topic, _valueB));
            Assert.True(_client.Produce(Topic, _valueB));
            Assert.False(_client.Produce(Topic, _valueB));
            Assert.Equal(2, _client.Statistics.Entered);
            Assert.Equal(0, _client.Statistics.Exited);
        }

        [Fact]
        public void TestPauseResume()
        {
            _client.Pause("tutu");
            _client.Resume("titi");

            _consumer.Verify(c => c.StopConsume("tutu", Partitions.All, Offsets.Now));
            _consumer.Verify(c => c.StopConsume("tutu", Partitions.All, Offsets.Now));
        }

        [Fact]
        public void TestSubscription()
        {
            _client.Subscribe("group", new[] { "topic" }, new ConsumerGroupConfiguration());
            
            _consumer.Verify(
                c => c.StartConsumeSubscription(It.IsAny<IConsumerGroup>(), It.Is<IEnumerable<string>>(l => l.Count() == 1 && l.Contains("topic"))),
                Times.Once);
        }

        [Fact]
        public void TestTimestampSetOnlyInV10Compat()
        {
            var date = new DateTime(2017, 1, 13, 23, 45, 21, 42, DateTimeKind.Utc);

            _client.Produce("toto", null, date);

            _producer.Verify(
                p =>
                    p.Route("toto", It.Is<Message>(m => m.TimeStamp == 0), It.IsAny<int>(),
                        It.IsAny<DateTime>()));

            Init(new Configuration
            {
                Seeds = "localhost:1",
                TaskScheduler = new CurrentThreadTaskScheduler(),
                Compatibility = Compatibility.V0_10_1
            });

            _client.Produce("toto", null, date);

            _producer.Verify(
                p =>
                    p.Route("toto", It.Is<Message>(m => m.TimeStamp == Timestamp.ToUnixTimestamp(date)), It.IsAny<int>(),
                        It.IsAny<DateTime>()));
        }

        [Fact]
        public void RaisesPartitionsAssignedEvent()
        {
            var assignments = new Dictionary<string, ISet<int>>();

            var eventRisen = false;

            _client.PartitionsAssigned += x => eventRisen = true;

            _consumer.Raise(x => x.PartitionsAssigned += null, assignments);

            Assert.True(eventRisen);
        }

        [Fact]
        public void RaisesPartitionsRevokedEvent()
        {
            var eventRisen = false;

            _client.PartitionsRevoked += () => eventRisen = true;

            _consumer.Raise(x => x.PartitionsRevoked += null);

            Assert.True(eventRisen);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}