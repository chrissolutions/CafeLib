using System;
using CafeLib.Kafka.Common;

namespace CafeLib.Kafka.Protocol
{
    public class Broker
    {
        public int BrokerId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public Uri Address => new Uri($"http://{Host}:{Port}");

        public static Broker FromStream(BigEndianBinaryReader stream)
        {
            return new Broker
                {
                    BrokerId = stream.ReadInt32(),
                    Host = stream.ReadInt16String(),
                    Port = stream.ReadInt32()
                };
        }
    }
}
