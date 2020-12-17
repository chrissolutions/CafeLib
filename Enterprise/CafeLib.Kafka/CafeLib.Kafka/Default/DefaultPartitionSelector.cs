using System;
using System.Collections.Concurrent;
using System.Linq;
using CafeLib.Kafka.Common;
using CafeLib.Kafka.Interfaces;
using CafeLib.Kafka.Protocol;

namespace CafeLib.Kafka.Default
{
    public class DefaultPartitionSelector : IPartitionSelector
    {
        private readonly ConcurrentDictionary<string, int> _roundRobinTracker = new ConcurrentDictionary<string, int>();

        public Partition Select(Topic topic, byte[] key)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (topic.Partitions.Count <= 0) throw new ApplicationException($"Topic ({topic.Name}) has no partitions.");

            //use round robin
            var partitions = topic.Partitions;
            if (key == null)
            {
                //use round robin
                var partitionIndex = _roundRobinTracker.AddOrUpdate(topic.Name, p => 0, (s, i) => (i + 1) % partitions.Count);
                return partitions[partitionIndex];
            }

            //use key hash
            var partitionId = Crc32Provider.Compute(key) % partitions.Count;
            var partition = partitions.FirstOrDefault(x => x.PartitionId == partitionId);

            if (partition == null)
                throw new InvalidPartitionException(
                    $"Hash function return partition id: {partitionId}, but the available partitions are:{string.Join(",", partitions.Select(x => x.PartitionId))}");

            return partition;
        }
    }
}