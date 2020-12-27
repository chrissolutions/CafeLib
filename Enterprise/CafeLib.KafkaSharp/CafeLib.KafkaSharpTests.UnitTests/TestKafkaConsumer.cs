using System.Collections.Generic;

namespace CafeLib.KafkaSharpTests.UnitTests
{
    public class TestKafkaConsumer
    {
        [Test]
        public void RaisesPartitionsAssignedEvent()
        {
            var clusterClientStub = CreateClusterClientStub();

            using (var sut = new KafkaConsumer<object, object>("ANYTOPIC", clusterClientStub.Object))
            {
                var assignments = new Dictionary<string, ISet<int>>();

                var eventRisen = false;

                sut.PartitionsAssigned += x => eventRisen = true;

                clusterClientStub.Raise(x => x.PartitionsAssigned += null, assignments);

                Assert.That(eventRisen, Is.True);
            }
        }

        [Test]
        public void RaisesPartitionsRevokedEvent()
        {
            var clusterClientStub = CreateClusterClientStub();
            using (var sut = new KafkaConsumer<object, object>("ANYTOPIC", clusterClientStub.Object))
            {
                var eventRisen = false;

                sut.PartitionsRevoked += () => eventRisen = true;

                clusterClientStub.Raise(x => x.PartitionsRevoked += null);

                Assert.That(eventRisen, Is.True);
            }
        }

        private Mock<IClusterClient> CreateClusterClientStub()
        {
            var stub = new Mock<IClusterClient>();

            stub.SetupGet(x => x.Messages).Returns(Observable.Empty<RawKafkaRecord>());

            return stub;
        }
    }
}