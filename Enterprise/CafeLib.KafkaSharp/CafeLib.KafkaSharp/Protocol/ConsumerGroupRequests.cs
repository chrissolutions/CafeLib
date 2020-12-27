﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Linq;
using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{

    #region JoinConsumerGroupRequest

    struct ConsumerGroupProtocolMetadata : IMemoryStreamSerializable
    {
        public short Version;
        public IEnumerable<string> Subscription;
        public byte[] UserData;

        #region Serialization

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, Version);
            Basics.WriteArray(stream, Subscription ?? Enumerable.Empty<string>(), Basics.SerializeString);
            Basics.SerializeBytes(stream, UserData);
        }

        #region Deserialization (for test)

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Version = BigEndianConverter.ReadInt16(stream);
            Subscription = Basics.DeserializeArray(stream, Basics.DeserializeString);
            UserData = Basics.DeserializeBytes(stream);
        }

        #endregion

        #endregion
    }

    struct ConsumerGroupProtocol : IMemoryStreamSerializable
    {
        public string ProtocolName;
        public ConsumerGroupProtocolMetadata ProtocolMetadata;

        #region Serialization

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, ProtocolName);
            var pm = ProtocolMetadata;
            Basics.WriteWithSize(stream, s => pm.Serialize(s, null, Basics.ApiVersion.Ignored));
        }

        #region Deserialization (for test)

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            ProtocolName = Basics.DeserializeString(stream);
            BigEndianConverter.ReadInt32(stream);
            ProtocolMetadata = new ConsumerGroupProtocolMetadata();
            ProtocolMetadata.Deserialize(stream, null, Basics.ApiVersion.Ignored);
        }

        #endregion

        #endregion
    }

    class JoinConsumerGroupRequest : ISerializableRequest
    {
        public string GroupId;
        public int SessionTimeout;
        public int RebalanceTimeout;
        public string MemberId;
        public IEnumerable<string> Subscription;

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId,
            object extra, Basics.ApiVersion version)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.JoinGroupRequest,
                version, null);
        }

        public void SerializeBody(ReusableMemoryStream stream, object extra, Basics.ApiVersion version)
        {
            Basics.SerializeString(stream, GroupId);
            BigEndianConverter.Write(stream, SessionTimeout);
            if (version > Basics.ApiVersion.V0)
            {
                BigEndianConverter.Write(stream, RebalanceTimeout);
            }
            Basics.SerializeString(stream, MemberId);
            Basics.SerializeString(stream, "consumer");
            var metadata = new[] // Only one protocol is supported
            {
                new ConsumerGroupProtocol
                {
                    ProtocolName = "kafka-sharp-consumer",
                    ProtocolMetadata =
                        new ConsumerGroupProtocolMetadata { Version = 0, Subscription = Subscription, UserData = null, }
                }
            };
            Basics.WriteArray(stream, metadata, (s, d) => d.Serialize(s, null, Basics.ApiVersion.Ignored));
        }
    }

    #endregion

    #region SyncConsumerGroupRequest

    struct PartitionAssignment : IMemoryStreamSerializable
    {
        public int Partition;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, Partition);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Partition = BigEndianConverter.ReadInt32(stream);
        }
    }

    struct ConsumerGroupMemberAssignment : IMemoryStreamSerializable
    {
        public short Version;
        public IEnumerable<TopicData<PartitionAssignment>> PartitionAssignments;
        public byte[] UserData;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, Version);
            Basics.WriteArray(stream, PartitionAssignments, Basics.ApiVersion.Ignored);
            Basics.SerializeBytes(stream, UserData);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Version = BigEndianConverter.ReadInt16(stream);
            PartitionAssignments = Basics.DeserializeArray<TopicData<PartitionAssignment>>(stream);
            UserData = Basics.DeserializeBytes(stream);
        }
    }

    struct ConsumerGroupAssignment : IMemoryStreamSerializable
    {
        public string MemberId;
        public ConsumerGroupMemberAssignment MemberAssignment;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, MemberId);
            var ma = MemberAssignment;
            Basics.WriteWithSize(stream, s => ma.Serialize(s, null, Basics.ApiVersion.Ignored));
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            MemberId = Basics.DeserializeString(stream);
            MemberAssignment = new ConsumerGroupMemberAssignment
            {
                PartitionAssignments = Enumerable.Empty<TopicData<PartitionAssignment>>()
            };
            if (BigEndianConverter.ReadInt32(stream) > 0)
            {
                MemberAssignment.Deserialize(stream, null, Basics.ApiVersion.Ignored);
            }
        }
    }

    class SyncConsumerGroupRequest : ISerializableRequest
    {
        public string GroupId;
        public int GenerationId;
        public string MemberId;
        public IEnumerable<ConsumerGroupAssignment> GroupAssignment;

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId,
            object _, Basics.ApiVersion __)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.SyncGroupRequest,
                Basics.ApiVersion.V0, null);
        }

        public void SerializeBody(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, GroupId);
            BigEndianConverter.Write(stream, GenerationId);
            Basics.SerializeString(stream, MemberId);
            Basics.WriteArray(stream, GroupAssignment);
        }
    }

    #endregion

    #region HeartbeatRequest

    class HeartbeatRequest : ISerializableRequest
    {
        public string GroupId;
        public int GenerationId;
        public string MemberId;

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId, object _, Basics.ApiVersion __)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.HeartbeatRequest,
                Basics.ApiVersion.V0, null);
        }

        public void SerializeBody(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, GroupId);
            BigEndianConverter.Write(stream, GenerationId);
            Basics.SerializeString(stream, MemberId);
        }
    }

    #endregion

    #region LeaveGroupRequest

    class LeaveGroupRequest : ISerializableRequest
    {
        public string GroupId;
        public string MemberId;

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId, object _, Basics.ApiVersion __)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.LeaveGroupRequest,
                Basics.ApiVersion.V0, null);
        }

        public void SerializeBody(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, GroupId);
            Basics.SerializeString(stream, MemberId);
        }
    }

    #endregion
}