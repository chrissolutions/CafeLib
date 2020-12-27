﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    class MetadataResponse
    {
        public BrokerMeta[] BrokersMeta;
        public TopicMeta[] TopicsMeta;

        public static MetadataResponse Deserialize(ReusableMemoryStream stream, object noextra)
        {
            return new MetadataResponse
            {
                BrokersMeta = Basics.DeserializeArray<BrokerMeta>(stream),
                TopicsMeta = Basics.DeserializeArray<TopicMeta>(stream)
            };
        }

        // Used only in tests
        public void Serialize(ReusableMemoryStream stream, object noextra)
        {
            Basics.WriteArray(stream, BrokersMeta, noextra, Basics.ApiVersion.Ignored);
            Basics.WriteArray(stream, TopicsMeta, noextra, Basics.ApiVersion.Ignored);
        }
    }
}
