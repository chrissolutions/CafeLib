// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System;
using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    internal struct CommonResponse<TPartitionData> : IMemoryStreamSerializable where TPartitionData : IMemoryStreamSerializable, new()
    {
        public TopicData<TPartitionData>[] TopicsResponse;

        public void Serialize(ReusableMemoryStream stream, object extra, Basics.ApiVersion version)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ReusableMemoryStream stream, object extra, Basics.ApiVersion version)
        {
            TopicsResponse = Basics.DeserializeArrayExtra<TopicData<TPartitionData>>(stream, extra, version);
        }
    }
}