﻿using System;
using System.Linq;
using System.Threading;

namespace CafeLib.KafkaSharpTests.UnitTests
{
    [TestFixture]
    internal class TestGeneral
    {
        private static ISerializer Serializer = new StringSerializer();

        private static ClusterClient InitCluster(Configuration configuration, ILogger logger, MetadataResponse metadata, bool forceErrors = false, bool forceConnectionErrors = false, int responseDelay = 0)
        {
            TestData.Reset();
            var cluster = new Cluster(
                configuration,
                logger,
                (h, p) =>
                    new Node(string.Format("[{0}:{1}]", h, p), () => new EchoConnectionMock(forceConnectionErrors, responseDelay),
                        new ScenarioSerializationMock(metadata, forceErrors), configuration, 1),
                null, null);
            return new ClusterClient(configuration, logger, cluster);
        }

        [Test]
        public void TestOneProduce()
        {
            var logger = new TestLogger();
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                ErrorStrategy = ErrorStrategy.Discard,
                Seeds = "localhost:1,localhost:2,localhost:3"
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            const int expectedLatency = 5;
            var cluster = InitCluster(configuration, logger, TestData.TestMetadataResponse, forceErrors: false, forceConnectionErrors: false, responseDelay: expectedLatency);

            cluster.Produce("topic1", "key", "value");
            SpinWait.SpinUntil(() => cluster.Statistics.Exited == 1);
            cluster.Dispose();
            var statistics = cluster.Statistics;
            Assert.AreEqual(1, statistics.Exited);
            Assert.AreEqual(1, statistics.SuccessfulSent);
            Assert.AreEqual(0, statistics.Errors);
            Assert.AreEqual(0, statistics.Expired);
            Assert.AreEqual(0, statistics.Discarded);
            Assert.AreEqual(0, statistics.NodeDead);
            Assert.GreaterOrEqual(statistics.ResponseReceived, 2); // 1 produce, 1 or more fetch metadata
            Assert.GreaterOrEqual(statistics.LatestRequestLatency, expectedLatency);
            Assert.GreaterOrEqual(statistics.RequestSent, 2); // 1 produce response, 1 or more fetch metadata response
            Assert.GreaterOrEqual(logger.InformationLog.Count(), 3); // Fetch metadata feedback
            Assert.AreEqual(0, logger.ErrorLog.Count());
            Assert.AreEqual(0, logger.WarningLog.Count());
        }

        void TestMultipleProduce(Configuration configuration)
        {
            var logger = new TestLogger();

            const int expectedLatency = 1;
            var cluster = InitCluster(configuration, logger, TestData.TestMetadataResponse, forceErrors: false, forceConnectionErrors: false, responseDelay: expectedLatency);

            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");

            SpinWait.SpinUntil(() => cluster.Statistics.Exited == 14);
            cluster.Dispose();
            var statistics = cluster.Statistics;
            Assert.AreEqual(14, statistics.Exited);
            Assert.AreEqual(14, statistics.SuccessfulSent);
            Assert.AreEqual(0, statistics.Errors);
            Assert.AreEqual(0, statistics.Expired);
            Assert.AreEqual(0, statistics.Discarded);
            Assert.AreEqual(0, statistics.NodeDead);
            Assert.GreaterOrEqual(statistics.ResponseReceived, 3); // 2 or more produce, 1 or more fetch metadata
            Assert.GreaterOrEqual(statistics.LatestRequestLatency, expectedLatency);
            Assert.GreaterOrEqual(statistics.RequestSent, 3); // 2 or more produce response, 1 or more fetch metadata response
            Assert.GreaterOrEqual(logger.InformationLog.Count(), 3); // Fetch metadata feedback
            Assert.AreEqual(0, logger.ErrorLog.Count());
            Assert.AreEqual(0, logger.WarningLog.Count());
        }

        [Test]
        public void TestMultipleProduce()
        {
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                ErrorStrategy = ErrorStrategy.Discard,
                Seeds = "localhost:1,localhost:2,localhost:3"
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            TestMultipleProduce(configuration);
        }

        [Test]
        public void TestMultipleProduceGlobalAccumulator()
        {
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                ErrorStrategy = ErrorStrategy.Discard,
                Seeds = "localhost:1,localhost:2,localhost:3",
                BatchStrategy = BatchStrategy.Global
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            TestMultipleProduce(configuration);
        }

        [Test]
        public void TestMultipleProduceConcurrencyOne()
        {
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                ErrorStrategy = ErrorStrategy.Discard,
                Seeds = "localhost:1,localhost:2,localhost:3",
                TaskScheduler = new ActionBlockTaskScheduler(1)
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            TestMultipleProduce(configuration);
        }

        [Test]
        [Ignore("Flaky")]
        public void TestMultipleProduceWithErrorsAndDiscard()
        {
            var logger = new TestLogger();
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                MinimumTimeBetweenRefreshMetadata = default(TimeSpan),
                ErrorStrategy = ErrorStrategy.Discard,
                Seeds = "localhost:1,localhost:2,localhost:3"
            };
            var cluster = InitCluster(configuration, logger, TestData.TestMetadataResponse, forceErrors: true);

            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");

            SpinWait.SpinUntil(() => cluster.Statistics.Exited == 14);
            cluster.Dispose();
            var statistics = cluster.Statistics;
            Assert.AreEqual(14, statistics.Exited);
            Assert.GreaterOrEqual(statistics.SuccessfulSent, 1);
            Assert.GreaterOrEqual(statistics.Errors, 0);
            Assert.AreEqual(0, statistics.Expired);
            Assert.GreaterOrEqual(statistics.Discarded, 1); // At least once an irrecoverable error
            Assert.AreEqual(0, statistics.NodeDead);
            Assert.GreaterOrEqual(statistics.ResponseReceived, 2); // 1 or more successful produce, 1 or more fetch metadata
            Assert.GreaterOrEqual(statistics.RequestSent, 3); // 2 or more produce response, 1 or more fetch metadata response
            Assert.GreaterOrEqual(logger.InformationLog.Count(), 3); // Fetch metadata feedback
            Assert.GreaterOrEqual(logger.ErrorLog.Count(), 1); // At least once an irrecoverable error
            Assert.AreEqual(2, logger.WarningLog.Count());
        }

        [Test]
        public void TestMultipleProduceWithNetworkErrorsAndRetry()
        {
            var logger = new TestLogger();
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                ErrorStrategy = ErrorStrategy.Retry,
                MaxRetry = -1,
                MaxSuccessiveNodeErrors = 10,
                Seeds = "localhost:1,localhost:2,localhost:3"
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            var cluster = InitCluster(configuration, logger, TestData.TestMetadataResponse, forceErrors: false, forceConnectionErrors: true);

            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");

            SpinWait.SpinUntil(() => cluster.Statistics.Exited == 14);
            cluster.Dispose();

            var statistics = cluster.Statistics;

            Assert.AreEqual(14, statistics.Exited);
            Assert.GreaterOrEqual(statistics.SuccessfulSent, 1);
            Assert.GreaterOrEqual(statistics.Errors, 1);
            Assert.AreEqual(0, statistics.Expired);
            Assert.AreEqual(0, statistics.Discarded); // only network errors and we retry
            Assert.AreEqual(0, statistics.NodeDead);
            Assert.GreaterOrEqual(statistics.ResponseReceived, 3); // 2 or more successful produce, 1 or more fetch metadata
            Assert.GreaterOrEqual(statistics.RequestSent, 3); // 2 or more produce response, 1 or more fetch metadata response
            Assert.GreaterOrEqual(logger.InformationLog.Count(), 3); // Fetch metadata feedback
            Assert.GreaterOrEqual(logger.ErrorLog.Count(), 1);
            Assert.AreEqual(0, logger.WarningLog.Count());
            Assert.GreaterOrEqual(statistics.MessageRetry, 2);
        }

        [Test]
        [Ignore("Flaky")]
        public void TestBigShake()
        {
            var logger = new TestLogger();
            var configuration = new Configuration
            {
                ProduceBatchSize = 10,
                ProduceBufferingTime = TimeSpan.FromMilliseconds(15),
                MinimumTimeBetweenRefreshMetadata = default(TimeSpan),
                ErrorStrategy = ErrorStrategy.Retry,
                Seeds = "localhost:1,localhost:2,localhost:3"
            };
            configuration.SerializationConfig.SetDefaultSerializers(Serializer, Serializer);
            var cluster = InitCluster(configuration, logger, TestData.TestMetadataResponse, forceErrors: true, forceConnectionErrors: true);

            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic2", "key", "value");
            cluster.Produce("topic1", "key", "value");
            cluster.Produce("topic3", "key", "value");
            cluster.Produce("topic1", "key", "value");

            SpinWait.SpinUntil(() => cluster.Statistics.Exited == 14);
            cluster.Dispose();
            var statistics = cluster.Statistics;
            Assert.AreEqual(14, statistics.Exited);
            Assert.GreaterOrEqual(statistics.SuccessfulSent, 1);
            Assert.GreaterOrEqual(statistics.Errors, 1);
            Assert.AreEqual(0, statistics.Expired);
            Assert.AreEqual(0, statistics.NodeDead);
            Assert.GreaterOrEqual(statistics.ResponseReceived, 3); // 2 or more successful produce, 1 or more fetch metadata
            Assert.GreaterOrEqual(statistics.RequestSent, 3); // 2 or more produce response, 1 or more fetch metadata response
            Assert.GreaterOrEqual(logger.InformationLog.Count(), 3); // Fetch metadata feedback
            Assert.GreaterOrEqual(logger.ErrorLog.Count(), 1);
            Assert.AreEqual(0, logger.WarningLog.Count());
        }
    }
}
