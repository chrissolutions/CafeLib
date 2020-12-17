﻿using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Kafka.Common;
using CafeLib.Kafka.Interfaces;

namespace CafeLib.Kafka.Protocol
{
    /// <summary>
    /// Class that represents both the request and the response from a kafka server of requesting a stored offset value
    /// for a given consumer group.  Essentially this part of the api allows a user to save/load a given offset position
    /// under any abritrary name.
    /// </summary>
    public class OffsetFetchRequest : BaseRequest, IKafkaRequest<OffsetFetchResponse>
    {
        public ApiKeyRequestType ApiKey => ApiKeyRequestType.OffsetFetch;
        public string ConsumerGroup { get; set; }
        public List<OffsetFetch> Topics { get; set; }

        public KafkaDataPayload Encode()
        {
            return EncodeOffsetFetchRequest(this);
        }

        protected KafkaDataPayload EncodeOffsetFetchRequest(OffsetFetchRequest request)
        {
            if (request.Topics == null) request.Topics = new List<OffsetFetch>();

            using var message = EncodeHeader(request);
            var topicGroups = request.Topics.GroupBy(x => x.Topic).ToList();

            message.Pack(ConsumerGroup, StringPrefixEncoding.Int16)
                .Pack(topicGroups.Count);

            foreach (var topicGroup in topicGroups)
            {
                var partitions = topicGroup.GroupBy(x => x.PartitionId).ToList();
                message.Pack(topicGroup.Key, StringPrefixEncoding.Int16)
                    .Pack(partitions.Count);

                foreach (var partition in partitions)
                {
                    foreach (var offset in partition)
                    {
                        message.Pack(offset.PartitionId);
                    }
                }
            }

            return new KafkaDataPayload
            {
                Buffer = message.Payload(),
                CorrelationId = request.CorrelationId,
                ApiKey = ApiKey
            };
        }

        public IEnumerable<OffsetFetchResponse> Decode(byte[] payload)
        {
            return DecodeOffsetFetchResponse(payload);
        }


        protected IEnumerable<OffsetFetchResponse> DecodeOffsetFetchResponse(byte[] data)
        {
            using var stream = new BigEndianBinaryReader(data);
            // ReSharper disable once UnusedVariable
            var correlationId = stream.ReadInt32();

            var topicCount = stream.ReadInt32();
            for (int i = 0; i < topicCount; i++)
            {
                var topic = stream.ReadInt16String();

                var partitionCount = stream.ReadInt32();
                for (int j = 0; j < partitionCount; j++)
                {
                    var response = new OffsetFetchResponse()
                    {
                        Topic = topic,
                        PartitionId = stream.ReadInt32(),
                        Offset = stream.ReadInt64(),
                        MetaData = stream.ReadInt16String(),
                        Error = stream.ReadInt16()
                    };
                    yield return response;
                }
            }
        }

    }

    public class OffsetFetch
    {
        /// <summary>
        /// The topic the offset came from.
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// The partition the offset came from.
        /// </summary>
        public int PartitionId { get; set; }
    }

    public class OffsetFetchResponse
    {
        /// <summary>
        /// The name of the topic this response entry is for.
        /// </summary>
        public string Topic;
        /// <summary>
        /// The id of the partition this response is for.
        /// </summary>
        public int PartitionId;
        /// <summary>
        /// The offset position saved to the server.
        /// </summary>
        public long Offset;
        /// <summary>
        /// Any arbitrary metadata stored during a CommitRequest.
        /// </summary>
        public string MetaData;
        /// <summary>
        /// Error code of exception that occured during the request.  Zero if no error.
        /// </summary>
        public int Error;

        public override string ToString()
        {
            return $"[OffsetFetchResponse TopicName={Topic}, PartitionID={PartitionId}, Offset={Offset}, MetaData={MetaData}, ErrorCode={Error}]";
        }
    }
}