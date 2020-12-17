using CafeLib.Kafka.Interfaces;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace CafeLib.Kafka.Model
{
    public class BrokerRoute
    {
        public string Topic { get; set; }
        public int PartitionId { get; set; }
        public IKafkaConnection Connection { get; set; }
        public override string ToString()
        {
            return $"{Connection.Endpoint.ServeUri} Topic:{Topic} PartitionId:{PartitionId}";
        }

        #region Equals Override...
        protected bool Equals(BrokerRoute other)
        {
            return string.Equals(Topic, other.Topic) && PartitionId == other.PartitionId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Topic != null ? Topic.GetHashCode() : 0) * 397) ^ PartitionId;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BrokerRoute)obj);
        }
        #endregion
    }
}