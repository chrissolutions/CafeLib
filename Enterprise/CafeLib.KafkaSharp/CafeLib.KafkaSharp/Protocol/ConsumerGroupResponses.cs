﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System.Linq;
using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{

    #region JoinConsumerGroupResponse

    struct GroupMember : IMemoryStreamSerializable
    {
        public string MemberId;
        public ConsumerGroupProtocolMetadata Metadata;

        #region Serialization

        #region Serialization (for test)

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Basics.SerializeString(stream, MemberId);
            var pm = Metadata;
            Basics.WriteWithSize(stream, s => pm.Serialize(s, null, Basics.ApiVersion.Ignored));
        }

        #endregion

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            MemberId = Basics.DeserializeString(stream);
            BigEndianConverter.ReadInt32(stream);
            Metadata = new ConsumerGroupProtocolMetadata();
            Metadata.Deserialize(stream, null, Basics.ApiVersion.Ignored);
        }

        #endregion
    }

    struct JoinConsumerGroupResponse : IMemoryStreamSerializable
    {
        public ErrorCode ErrorCode;
        public int GenerationId;
        public string GroupProtocol;
        public string LeaderId;
        public string MemberId;
        public GroupMember[] GroupMembers;

        #region Serialization

        #region Serialization (for test)

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, (short) ErrorCode);
            BigEndianConverter.Write(stream, GenerationId);
            Basics.SerializeString(stream, GroupProtocol);
            Basics.SerializeString(stream, LeaderId);
            Basics.SerializeString(stream, MemberId);
            Basics.WriteArray(stream, GroupMembers, (s, m) => m.Serialize(s, null, Basics.ApiVersion.Ignored));
        }

        #endregion

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
            GenerationId = BigEndianConverter.ReadInt32(stream);
            GroupProtocol = Basics.DeserializeString(stream);
            LeaderId = Basics.DeserializeString(stream);
            MemberId = Basics.DeserializeString(stream);
            GroupMembers = Basics.DeserializeArray<GroupMember>(stream);
        }

        #endregion
    }

    #endregion

    #region SyncConsumerGroupResponse

    struct SyncConsumerGroupResponse : IMemoryStreamSerializable
    {
        public ErrorCode ErrorCode;
        public ConsumerGroupMemberAssignment MemberAssignment;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, (short) ErrorCode);
            var ma = MemberAssignment;
            Basics.WriteWithSize(stream, s => ma.Serialize(s, null, Basics.ApiVersion.Ignored));
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
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

    #endregion

    #region HeartbeatResponse / LeaveGroupResponse

    struct SimpleResponse : IMemoryStreamSerializable
    {
        public ErrorCode ErrorCode;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, (short) ErrorCode);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
        }
    }

    #endregion
}
