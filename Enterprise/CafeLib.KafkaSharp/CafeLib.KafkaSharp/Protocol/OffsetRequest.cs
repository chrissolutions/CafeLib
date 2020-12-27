﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    struct OffsetRequest : ISerializableRequest
    {
        public IEnumerable<TopicData<OffsetPartitionData>> TopicsData;

        #region Serialization

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId, object _, Basics.ApiVersion version)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.OffsetRequest, version, null);
        }

        public void SerializeBody(ReusableMemoryStream stream, object _, Basics.ApiVersion version)
        {
            stream.Write(Basics.MinusOne32, 0, 4); // ReplicaId, non clients that are not a broker must use -1
            Basics.WriteArray(stream, TopicsData, version);
        }

        #endregion
    }

    struct OffsetPartitionData : IMemoryStreamSerializable
    {
        public int Partition;
        public int MaxNumberOfOffsets;
        public long Time;

        #region Serialization

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion version)
        {
            BigEndianConverter.Write(stream, Partition);
            BigEndianConverter.Write(stream, Time);
            if (version == Basics.ApiVersion.V0)
            {
                BigEndianConverter.Write(stream, MaxNumberOfOffsets);
            }
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion version)
        {
            Partition = BigEndianConverter.ReadInt32(stream);
            Time = BigEndianConverter.ReadInt64(stream);
            if (version == Basics.ApiVersion.V0)
            {
                MaxNumberOfOffsets = BigEndianConverter.ReadInt32(stream);
            }
        }

        #endregion
    }
}
