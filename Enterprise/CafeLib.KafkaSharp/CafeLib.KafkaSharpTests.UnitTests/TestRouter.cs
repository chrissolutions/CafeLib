﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CafeLib.KafkaSharpTests.UnitTests
{
    [TestFixture]
    class TestProducer
    {
        private static readonly Pool<ReusableMemoryStream> Pool =
            new Pool<ReusableMemoryStream>(() => new ReusableMemoryStream(Pool), (m, b) => { m.SetLength(0); });
        private NodeMock[] _nodes;
        private ClusterMock _cluster;
        private Dictionary<string, Partition[]> _routes;
        private ProduceRouter _produceRouter;
        private Dictionary<string, int> _messagesSentByTopic;
        private int MessagesEnqueued;
        private int MessagesReEnqueued;
        private int MessagesRouted;
        private int MessagesExpired;
        private int MessagesPostponed;
        private int RoutingTableRequired;

        private AsyncCountdownEvent _finished;

        private void InitRouter(ProduceRouter produceRouter)
        {
            _produceRouter = produceRouter;
            _produceRouter.MessageEnqueued += _ =>
            {
                ++MessagesEnqueued;
                if (_finished != null) _finished.Signal();
            };
            _produceRouter.MessageReEnqueued += _ =>
            {
                ++MessagesReEnqueued;
                if (_finished != null) _finished.Signal();
            };
            _produceRouter.MessageExpired += (t, m) =>
            {
                ++MessagesExpired;
                if (_finished != null) _finished.Signal();
            };
            _produceRouter.MessageRouted += _ =>
            {
                ++MessagesRouted;
                if (_finished != null) _finished.Signal();
            };
            _produceRouter.MessagePostponed += _ =>
            {
                ++MessagesPostponed;
                if (_finished != null) _finished.Signal();
            };
            _produceRouter.RoutingTableRequired += () =>
            {
                ++RoutingTableRequired;
                if (_finished != null) _finished.Signal();
            };
        }

        [SetUp]
        public void SetUp()
        {
            SetUp(new Configuration{TaskScheduler = new CurrentThreadTaskScheduler()});
        }

        private void SetUp(Configuration config)
        {
            _messagesSentByTopic = new Dictionary<string, int>
                {
                    {"test", 0},
                    {"test2", 0},
                    {"test1p", 0},
                    {"test2p", 0},
                    {"testallp", 0}
                };
            _nodes = new NodeMock[5];
            for (int i = 0; i < _nodes.Length; ++i)
            {
                _nodes[i] = new NodeMock();
                _nodes[i].MessageReceived += t => _messagesSentByTopic[t] += 1; // No need to use interlocked, NodeMock is synchronous
            }

            _routes = new Dictionary<string, Partition[]>
                {
                    {"test1p", new[] {new Partition {Id = 0, Leader = _nodes[0]}}},
                    {"test2p", new[] {new Partition {Id = 0, Leader = _nodes[0]}, new Partition {Id = 1, Leader = _nodes[1]}}},
                    {"testallp", new[]
                        {
                            new Partition {Id = 0, Leader = _nodes[0]},
                            new Partition {Id = 1, Leader = _nodes[1]},
                            new Partition {Id = 2, Leader = _nodes[2]},
                            new Partition {Id = 3, Leader = _nodes[3]},
                            new Partition {Id = 4, Leader = _nodes[4]},
                        }}
                };

            _cluster = new ClusterMock(_routes);

            MessagesEnqueued = MessagesExpired = MessagesReEnqueued = MessagesRouted = MessagesPostponed = RoutingTableRequired = 0;
            InitRouter(new ProduceRouter(_cluster, config, Pool));
            _finished = null;
        }

        [TearDown]
        public void TearDown()
        {
            _nodes = null;
            _cluster = null;
        }

        [Test]
        public async Task TestMessagesAreSent()
        {
            await TestMessagesAreSentCommon();
        }

        [Test]
        public async Task TestSerializeOnProduceAreSent()
        {
            SetUp(new Configuration
            {
                TaskScheduler = new CurrentThreadTaskScheduler(),
                SerializationConfig = new SerializationConfig { SerializeOnProduce = true }
            });

            await TestMessagesAreSentCommon();
        }

        private async Task TestMessagesAreSentCommon()
        {
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.Route("test2p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.Route("test2p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.Route("testallp", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.Route("testallp", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));

            await _produceRouter.Stop();

            Assert.AreEqual(2, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(2, _messagesSentByTopic["test2p"]);
            Assert.AreEqual(2, _messagesSentByTopic["testallp"]);

            CheckCounters(expectedMessagesEnqueued: 6, expectedMessagesReEnqueued: 0, expectedMessagesRouted: 6, expectedMessagesExpired: 0, expectedRoutingTableRequired: 1);
        }

        [Test]
        public void TestSerializeOnProduce()
        {
            var node = new Mock<INode>();
            node.Setup(n => n.Produce(It.IsAny<ProduceMessage>())).Returns(true);
            var cluster = new Mock<ICluster>();
            cluster.Setup(c => c.RequireNewRoutingTable())
                .Returns(() =>
                    Task.FromResult(new RoutingTable(new Dictionary<string, Partition[]>
                    {
                        {"toto", new[] {new Partition {Id = 0, Leader = node.Object}}}
                    })));
            var producer = new ProduceRouter(cluster.Object,
                new Configuration
                {
                    TaskScheduler = new CurrentThreadTaskScheduler(),
                    SerializationConfig = new SerializationConfig {SerializeOnProduce = true}
                }, Pool);

            var key = new byte[] {65, 66}; // 'A', 'B'
            var value = new byte[] {67, 68}; // 'C', 'D'
            producer.Route("toto", new Message {Key = key, Value = value}, Partitions.Any,
                DateTime.UtcNow.AddMinutes(42));
            node.Verify(
                n =>
                    n.Produce(
                        It.Is<ProduceMessage>(
                            p =>
                                p.Message.SerializedKeyValue != null && p.Message.Key != null && p.Message.Value == null)));
        }

        [Test]
        public void TestFilterMinInSyncNotEnough()
        {
            var node = new Mock<INode>();
            node.Setup(n => n.Produce(It.IsAny<ProduceMessage>())).Returns(true);
            var cluster = new Mock<ICluster>();
            cluster.Setup(c => c.RequireNewRoutingTable())
                .Returns(() =>
                    Task.FromResult(new RoutingTable(new Dictionary<string, Partition[]>
                    {
                        {"toto", new[] {new Partition {Id = 0, Leader = node.Object, NbIsr = 1}}}
                    })));
            var producer = new ProduceRouter(cluster.Object,
                new Configuration
                {
                    TaskScheduler = new CurrentThreadTaskScheduler(),
                    SerializationConfig = new SerializationConfig { SerializeOnProduce = true },
                    MinInSyncReplicas = 2
                }, Pool);

            var key = new byte[] { 65, 66 }; // 'A', 'B'
            var value = new byte[] { 67, 68 }; // 'C', 'D'
            producer.Route("toto", new Message { Key = key, Value = value }, Partitions.Any,
                DateTime.UtcNow.AddMinutes(42));
            node.Verify(n => n.Produce(It.IsAny<ProduceMessage>()), Times.Never());
        }

        [Test]
        public void TestFilterMinInSyncEnough()
        {
            var node = new Mock<INode>();
            node.Setup(n => n.Produce(It.IsAny<ProduceMessage>())).Returns(true);
            var cluster = new Mock<ICluster>();
            cluster.Setup(c => c.RequireNewRoutingTable())
                .Returns(() =>
                    Task.FromResult(new RoutingTable(new Dictionary<string, Partition[]>
                    {
                        {"toto", new[] {new Partition {Id = 0, Leader = node.Object, NbIsr = 2}}}
                    })));
            var producer = new ProduceRouter(cluster.Object,
                new Configuration
                {
                    TaskScheduler = new CurrentThreadTaskScheduler(),
                    SerializationConfig = new SerializationConfig { SerializeOnProduce = true },
                    MinInSyncReplicas = 2
                }, Pool);

            var key = new byte[] { 65, 66 }; // 'A', 'B'
            var value = new byte[] { 67, 68 }; // 'C', 'D'
            producer.Route("toto", new Message { Key = key, Value = value }, Partitions.Any,
                DateTime.UtcNow.AddMinutes(42));
            node.Verify(n => n.Produce(It.IsAny<ProduceMessage>()), Times.Once());
        }

        [Test]
        public void TestMessageAreSent_GlobalBatching()
        {
            var name = "n1";
            var node = new Mock<INode>();
            node.SetupGet(n => n.Name).Returns(name);
            node.Setup(n => n.Equals(It.IsAny<INode>())).Returns((INode other) => name == other.Name);
            node.Setup(n => n.GetHashCode()).Returns(name.GetHashCode);
            node.Setup(n => n.Post(It.IsAny<IBatchByTopicByPartition<ProduceMessage>>())).Returns(true);
            var cluster = new Mock<ICluster>();
            cluster.Setup(c => c.RequireNewRoutingTable())
                .Returns(() =>
                    Task.FromResult(new RoutingTable(new Dictionary<string, Partition[]>
                    {
                        {"toto", new[] {new Partition {Id = 0, Leader = node.Object}}}
                    })));
            var producer = new ProduceRouter(cluster.Object,
                new Configuration
                {
                    BatchStrategy = BatchStrategy.Global,
                    ProduceBatchSize = 2,
                    ProduceBufferingTime = TimeSpan.FromDays(2), // To make sure there's no race on batch tick
                    TaskScheduler = new CurrentThreadTaskScheduler()
                }, Pool);

            producer.Route("toto", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(42));
            // The second Route will tick a batch. All of this is done on the test thread thanks
            // to CurrentThreadScheduler and large ProduceBufferingTime.
            producer.Route("toto", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(42));
            producer.Route("toto", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(42));

            node.Verify(n => n.Post(It.Is<IBatchByTopicByPartition<ProduceMessage>>(b => b.Count == 2)));
        }

        private void CheckCounters(int expectedMessagesEnqueued = 0, int expectedMessagesReEnqueued = 0, int expectedMessagesRouted = 0,
            int expectedMessagesExpired = 0, int expectedMessagesPostponed = 0, int expectedRoutingTableRequired = 0)
        {
            Assert.AreEqual(expectedMessagesEnqueued, MessagesEnqueued);
            Assert.AreEqual(expectedMessagesReEnqueued, MessagesReEnqueued);
            Assert.AreEqual(expectedMessagesRouted, MessagesRouted);
            Assert.AreEqual(expectedMessagesExpired, MessagesExpired);
            Assert.AreEqual(expectedMessagesPostponed, MessagesPostponed);
            Assert.AreEqual(expectedRoutingTableRequired, RoutingTableRequired);
        }

        [Test]
        public void TestAcknowledgementNoError()
        {
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 0
                                }
                            }
                        }
                    }
                }},
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test", 0, new Message(),
                            DateTime.UtcNow.AddDays(1))
                    })

            };

            _TestAcknowledgementNoError(acknowledgement, 1);
        }

        [Test]
        public void TestNotEnoughReplicasAfterAppendIgnored()
        {
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NotEnoughReplicasAfterAppend,
                                    Offset = 0,
                                    Partition = 0
                                }
                            }
                        }
                    }
                }},
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test", 0, new Message(),
                            DateTime.UtcNow.AddDays(1))
                    })

            };

            _TestAcknowledgementNoError(acknowledgement, 1);
        }

        [Test]
        public void TestProduceReplicaNotAvailableIsNotAnError()
        {
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.ReplicaNotAvailable,
                                    Offset = 0,
                                    Partition = 0
                                }
                            }
                        }
                    }
                }},
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test", 0, new Message(),
                            DateTime.UtcNow.AddDays(1))
                    })

            };

            _TestAcknowledgementNoError(acknowledgement, 1);
        }

        [Test]
        public void TestAcknowledgementMultipleNoError()
        {
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 0
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 1
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 2
                                }
                            }
                        }
                    }
                }},
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test", 0, new Message(),
                            DateTime.UtcNow.AddDays(1)),
                        ProduceMessage.New("test", 1, new Message(),
                            DateTime.UtcNow.AddDays(1)),
                        ProduceMessage.New("test", 2, new Message(),
                            DateTime.UtcNow.AddDays(1)),
                        ProduceMessage.New("test2", 0, new Message(),
                            DateTime.UtcNow.AddDays(1)),
                        ProduceMessage.New("test2", 1, new Message(),
                            DateTime.UtcNow.AddDays(1))
                    })

            };

            _TestAcknowledgementNoError(acknowledgement, 5);
        }

        public void _TestAcknowledgementNoError(ProduceAcknowledgement acknowledgement, int expected)
        {
            var ev = new ManualResetEvent(false);
            int rec = 0;
            int success = 0;
            int discarded = 0;
            _produceRouter.MessagesAcknowledged += (t, i) =>
            {
                Interlocked.Add(ref success, i);
                if (Interlocked.Add(ref rec, i) == expected)
                {
                    ev.Set();
                }
            };
            _produceRouter.MessageDiscarded += (t, m) =>
            {
                Interlocked.Increment(ref discarded);
                if (Interlocked.Increment(ref rec) == expected)
                {
                    ev.Set();
                }
            };

            _produceRouter.Acknowledge(acknowledgement);

            ev.WaitOne();
            Assert.AreEqual(expected, rec);
            Assert.AreEqual(expected, success);
            Assert.AreEqual(0, discarded);
        }

        [Test]
        public async Task TestAcknowledgementResponseNoneProduceWasNotSent()
        {
            _finished = new AsyncCountdownEvent(3);
            var acknowledgement = new ProduceAcknowledgement
            {
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {ProduceMessage.New("test1p", 0, new Message(), DateTime.UtcNow.AddDays(1))})
            };
            _produceRouter.Acknowledge(acknowledgement);
            await _finished.WaitAsync();
            CheckCounters(expectedMessagesReEnqueued: 1, expectedMessagesRouted: 1, expectedRoutingTableRequired: 1);
        }

        [Test]
        public async Task TestAcknowledgementResponseNoneProduceWasSentDiscard()
        {
            _finished = new AsyncCountdownEvent(1);
            int discarded = 0;
            _produceRouter.MessageDiscarded += (t, m) =>
                {
                    discarded += 1;
                    _finished.Signal();
                };
            var acknowledgement = new ProduceAcknowledgement
            {
                OriginalBatch = new TestBatchByTopicByPartition(new[]
                {
                    ProduceMessage.New("test1p", 0, new Message(),
                        DateTime.UtcNow.AddDays(1))
                }),
                ReceiveDate = DateTime.UtcNow
            };
            _produceRouter.Acknowledge(acknowledgement);
            await _finished.WaitAsync();
            Assert.AreEqual(1, discarded);
        }

        [Test]
        public async Task TestAcknowledgementResponseNoneProduceWasSentRetry()
        {
            InitRouter(new ProduceRouter(_cluster, new Configuration{ErrorStrategy = ErrorStrategy.Retry}, Pool));
            _finished = new AsyncCountdownEvent(3);
            var acknowledgement = new ProduceAcknowledgement
            {
                OriginalBatch = new TestBatchByTopicByPartition(new[]
                {
                    ProduceMessage.New("test1p", 0, new Message(),
                        DateTime.UtcNow.AddDays(1))
                }),
                ReceiveDate = DateTime.UtcNow
            };
            _produceRouter.Acknowledge(acknowledgement);
            await _finished.WaitAsync();
            CheckCounters(expectedMessagesReEnqueued: 1, expectedMessagesRouted: 1, expectedRoutingTableRequired: 1);
        }

        [Test]
        public void TestNonRecoverableErrorsAreDiscarded()
        {
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 0
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.MessageSizeTooLarge,
                                    Offset = 0,
                                    Partition = 1
                                }
                            }
                        }
                    }
                }},
                OriginalBatch = new TestBatchByTopicByPartition(new[]

                {
                    ProduceMessage.New("test", 0, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 1, new Message(),
                        DateTime.UtcNow.AddDays(1))
                })
            };

            var ev = new ManualResetEvent(false);
            int rec = 0;
            int success = 0;
            int discarded = 0;
            _produceRouter.MessagesAcknowledged += (t, i) =>
            {
                Interlocked.Add(ref success, i);
                if (Interlocked.Add(ref rec, i) == 2)
                {
                    ev.Set();
                }
            };
            _produceRouter.MessageDiscarded += (t, m) =>
            {
                Interlocked.Increment(ref discarded);
                if (Interlocked.Increment(ref rec) == 2)
                {
                    ev.Set();
                }
            };

            _produceRouter.Acknowledge(acknowledgement);

            ev.WaitOne();
            Assert.AreEqual(2, rec);
            Assert.AreEqual(1, success);
            Assert.AreEqual(1, discarded);
        }

        [Test]
        public void TestProduceRecoverableErrorsAreRerouted()
        {
            SetUp(new Configuration{TaskScheduler = new CurrentThreadTaskScheduler(), RetryIfNotEnoughReplicasAfterAppend = true});
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 0
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NotLeaderForPartition,
                                    Offset = 0,
                                    Partition = 1
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.LeaderNotAvailable,
                                    Offset = 0,
                                    Partition = 2
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.RequestTimedOut,
                                    Offset = 0,
                                    Partition = 3
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.UnknownTopicOrPartition,
                                    Offset = 0,
                                    Partition = 4
                                },
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NotEnoughReplicasAfterAppend,
                                    Offset = 0,
                                    Partition = 5
                                },
                            }
                        }
                    }
                }},
                OriginalBatch = new TestBatchByTopicByPartition(new[]
                {
                    ProduceMessage.New("test", 0, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 1, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 2, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 3, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 4, new Message(),
                        DateTime.UtcNow.AddDays(1)),
                    ProduceMessage.New("test", 5, new Message(),
                        DateTime.UtcNow.AddDays(1))
                })
            };

            const int exp = 6;
            var ev = new ManualResetEvent(false);
            int rec = 0;
            int success = 0;
            int discarded = 0;
            int rerouted = 0;
            int timeoutError = 0;

            _produceRouter.MessageReEnqueued += t =>
            {
                Interlocked.Increment(ref rerouted);
                if (Interlocked.Increment(ref rec) == exp)
                {
                    ev.Set();
                }
            };
            _produceRouter.MessagesAcknowledged += (t, i) =>
            {
                Interlocked.Add(ref success, i);
                if (Interlocked.Add(ref rec, i) == exp)
                {
                    ev.Set();
                }
            };
            _produceRouter.MessageDiscarded += (t, m) =>
            {
                Interlocked.Increment(ref discarded);
                if (Interlocked.Increment(ref rec) == exp)
                {
                    ev.Set();
                }
            };
            _produceRouter.BrokerTimeoutError += (t) =>
            {
                Interlocked.Increment(ref timeoutError);
            };

            _produceRouter.Acknowledge(acknowledgement);

            ev.WaitOne();

            // Check counts
            Assert.AreEqual(6, rec);
            Assert.AreEqual(1, success);
            Assert.AreEqual(0, discarded);
            Assert.AreEqual(5, rerouted);
            Assert.AreEqual(1, timeoutError);

            // Check logs
            var warnings =_cluster.Logger as TestLogger;
            var recov = warnings.WarningLog.Where(s => s.Contains("Recoverable")).ToArray();
            var ignore = warnings.WarningLog.Where(s => s.Contains("Will ignore")).ToArray();
            Assert.AreEqual(5, recov.Length);
            Assert.AreEqual(5, ignore.Length);
            for (int i = 1; i <= 5; ++i)
            {
                Assert.IsTrue(recov[i - 1].Contains("partition: " + i));
                Assert.IsTrue(ignore[i - 1].Contains("partition: " + i));
            }
        }

        [Test]
        public void TestMaxRetry()
        {
            int discarded = 0;
            _produceRouter.MessageDiscarded += (t, m) => discarded += 1;
            var pm = ProduceMessage.New("test1p", Partitions.Any, new Message(), DateTime.UtcNow.AddMinutes(5));
            pm.Retried = 42;
            _produceRouter.ReEnqueueAfterError(pm);
            Assert.AreEqual(1, discarded);
        }

        [Test]
        public void TestUnlimitedRetry()
        {
            SetUp(new Configuration
            {
                MaxRetry = -1
            });

            int discarded = 0;
            _produceRouter.MessageDiscarded += (t, m) => discarded += 1;
            var pm = ProduceMessage.New("test1p", Partitions.Any, new Message(), DateTime.UtcNow.AddMinutes(5));
            _produceRouter.ReEnqueueAfterError(pm);
            _produceRouter.ReEnqueueAfterError(pm);
            pm.Retried = int.MaxValue;
            _produceRouter.ReEnqueueAfterError(pm);
            Assert.AreEqual(0, discarded);
        }

        [Test]
        public async Task TestExpiredMessagesAreNotRouted()
        {
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMilliseconds(-1));
            _produceRouter.Route("test2p", new Message(), Partitions.Any, DateTime.UtcNow.AddMilliseconds(-1));

            await _produceRouter.Stop();

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(0, _messagesSentByTopic["test2p"]);
            CheckCounters(expectedMessagesEnqueued: 2, expectedMessagesExpired: 2);
        }

        [Test]
        public async Task TestNoMessageIsSentAfterStop()
        {
            int discarded = 0;
            _produceRouter.MessageDiscarded += (t, m) => discarded += 1;

            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            await _produceRouter.Stop();
            Assert.AreEqual(1, _messagesSentByTopic["test1p"]);

            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            _produceRouter.ReEnqueueAfterError(ProduceMessage.New("test1p", Partitions.Any, new Message(), DateTime.UtcNow.AddMinutes(5)));
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(1, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(2, discarded);
            CheckCounters(expectedMessagesEnqueued: 1, expectedMessagesRouted: 1, expectedRoutingTableRequired: 1);
        }

        [Test]
        public async Task TestMessagesArePostponedIfThereAreNoPartitions_AndSentWhenPartitionsBecomeAvailable()
        {
            _cluster.Partitions = new Dictionary<string, Partition[]>();

            _finished = new AsyncCountdownEvent(8);
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessagePostponed, 1x RoutingTableRequired
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessagePostponed, no routing table required because postponed
            _produceRouter.Route("test2p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessagePostponed, 1x RoutingTableRequired
            await _finished.WaitAsync();

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(0, _messagesSentByTopic["test2p"]);
            CheckCounters(expectedMessagesEnqueued: 3, expectedMessagesPostponed: 3, expectedRoutingTableRequired: 2);

            _finished = new AsyncCountdownEvent(6);
            _cluster.Partitions = _routes;
            // Advance a little the date of the new routing table, to be sure it's greater than the current one
            _produceRouter.ChangeRoutingTable(new RoutingTable(_routes){LastRefreshed = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(1))}); // => 3x Renqueued, 3x Routed
            await _finished.WaitAsync();

            Assert.AreEqual(2, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(1, _messagesSentByTopic["test2p"]);
            CheckCounters(expectedMessagesEnqueued: 3, expectedMessagesReEnqueued: 3, expectedMessagesRouted: 3, expectedMessagesPostponed: 3, expectedRoutingTableRequired: 2);
        }

        [Test]
        public void TestMaxPostponed()
        {
            SetUp(new Configuration
            {
                TaskScheduler = new CurrentThreadTaskScheduler(),
                MaxPostponedMessages = 1
            });
            int discarded = 0;
            _produceRouter.MessageDiscarded += (t, m) => discarded += 1;

            _cluster.Partitions = new Dictionary<string, Partition[]>();
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessagePostponed
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessageDiscarded because max postponed reached
            _produceRouter.Route("test2p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5)); // => 1x MessageEnqueued, 1x MessageDiscarded because max postponed reached

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            Assert.AreEqual(0, _messagesSentByTopic["test2p"]);
            Assert.AreEqual(2, discarded);
        }

        [Test]
        public async Task TestPostponedMessagesAreNotReEnqueuedIfExpired()
        {
            _cluster.Partitions = new Dictionary<string, Partition[]>();

            _finished = new AsyncCountdownEvent(3);
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMilliseconds(50));
            await _finished.WaitAsync();

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            CheckCounters(expectedMessagesEnqueued: 1, expectedMessagesPostponed: 1, expectedRoutingTableRequired: 1);

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            //trigger check postponed
            _cluster.Partitions = _routes;
            _produceRouter.ChangeRoutingTable(new RoutingTable(_routes));

            await _produceRouter.Stop();

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            CheckCounters(expectedMessagesEnqueued: 1, expectedMessagesExpired: 1, expectedMessagesPostponed: 1, expectedRoutingTableRequired: 1);
        }

        [Test]
        public async Task TestPostponedExpiredMessagesAreRemovedWhenNoPartition()
        {
            _cluster.Partitions = new Dictionary<string, Partition[]>();

            _finished = new AsyncCountdownEvent(5);
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMilliseconds(50));
            _produceRouter.Route("test1p", new Message(), Partitions.Any, DateTime.UtcNow.AddMinutes(5));
            await _finished.WaitAsync();

            Assert.AreEqual(0, _messagesSentByTopic["test1p"]);
            CheckCounters(expectedMessagesEnqueued: 2, expectedMessagesPostponed: 2, expectedRoutingTableRequired: 1);
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            //trigger check postponed
            _finished = new AsyncCountdownEvent(3);
            _cluster.Partitions = _routes;
            _produceRouter.ChangeRoutingTable(new RoutingTable(_routes)); // => 1x renqueue, 1x routed, 1x expired
            await _finished.WaitAsync();

            Assert.AreEqual(1, _messagesSentByTopic["test1p"]);
            CheckCounters(expectedMessagesEnqueued: 2, expectedMessagesReEnqueued: 1, expectedMessagesRouted: 1, expectedMessagesExpired: 1, expectedMessagesPostponed: 2, expectedRoutingTableRequired: 1);
        }

        [Test]
        public void TestDisposableMessagesAreDisposed()
        {
            var key = new Mock<IDisposable>();
            var value = new Mock<IDisposable>();
            var acknowledgement = new ProduceAcknowledgement
            {
                ProduceResponse = new ProduceResponse { ProducePartitionResponse = new CommonResponse<ProducePartitionResponse>()
                {
                    TopicsResponse = new[]
                    {
                        new TopicData<ProducePartitionResponse>
                        {
                            TopicName = "test",
                            PartitionsData = new[]
                            {
                                new ProducePartitionResponse
                                {
                                    ErrorCode = ErrorCode.NoError,
                                    Offset = 0,
                                    Partition = 0
                                }
                            }
                        }
                    }
                }},
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test", 0, new Message {Key = key.Object, Value = value.Object},
                            DateTime.UtcNow.AddDays(1))
                    })
            };

            _produceRouter.Acknowledge(acknowledgement);

            key.Verify(k => k.Dispose(), Times.Once());
            value.Verify(v => v.Dispose(), Times.Once());
        }

        [Test]
        public void TestDisposableMessagesDiscardedAreNotDisposed()
        {
            var key = new Mock<IDisposable>();
            var value = new Mock<IDisposable>();
            var acknowledgement = new ProduceAcknowledgement
            {
                OriginalBatch = new TestBatchByTopicByPartition(new[]
                {
                    ProduceMessage.New("test1p", 0, new Message {Key = key.Object, Value = value.Object},
                        DateTime.UtcNow.AddDays(1))
                }),
                ReceiveDate = DateTime.UtcNow
            };
            _produceRouter.Acknowledge(acknowledgement);

            key.Verify(k => k.Dispose(), Times.Never());
            value.Verify(v => v.Dispose(), Times.Never());
        }

        [Test]
        public void TestThrottled()
        {
            var node = new Mock<INode>();
            node.Setup(n => n.Produce(It.IsAny<ProduceMessage>())).Returns(true);
            var cluster = new Mock<ICluster>();
            cluster.Setup(c => c.RequireNewRoutingTable())
                .Returns(() =>
                    Task.FromResult(new RoutingTable(new Dictionary<string, Partition[]>
                    {
                        {"toto", new[] {new Partition {Id = 0, Leader = node.Object, NbIsr = 1}}}
                    })));
            var producer = new ProduceRouter(cluster.Object,
                new Configuration { TaskScheduler = new CurrentThreadTaskScheduler() }, Pool);

            int throttled = -1;
            producer.Throttled += t => throttled = t;

            var key = new Mock<IDisposable>();
            var value = new Mock<IDisposable>();
            var acknowledgement = new ProduceAcknowledgement
            {
                OriginalBatch =
                    new TestBatchByTopicByPartition(new[]
                    {
                        ProduceMessage.New("test1p", 0, new Message { Key = key.Object, Value = value.Object },
                            DateTime.UtcNow.AddDays(1))
                    }),
                ReceiveDate = DateTime.UtcNow,
                ProduceResponse =
                    new ProduceResponse
                    {
                        ThrottleTime = 42,
                        ProducePartitionResponse =
                            new CommonResponse<ProducePartitionResponse>
                            {
                                TopicsResponse =
                                    new[]
                                    {
                                        new TopicData<ProducePartitionResponse>
                                        {
                                            TopicName = "test1p",
                                            PartitionsData = new[] { new ProducePartitionResponse() }
                                        },
                                    }
                            }
                    }
            };
            producer.Acknowledge(acknowledgement);

            Assert.AreEqual(42, throttled);
        }
    }
}
