﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    struct GroupCoordinatorResponse : IMemoryStreamSerializable
    {
        public ErrorCode ErrorCode;
        public int CoordinatorId;
        public string CoordinatorHost;
        public int CoordinatorPort;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, (short) ErrorCode);
            BigEndianConverter.Write(stream, CoordinatorId);
            Basics.SerializeString(stream, CoordinatorHost);
            BigEndianConverter.Write(stream, CoordinatorPort);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
            CoordinatorId = BigEndianConverter.ReadInt32(stream);
            CoordinatorHost = Basics.DeserializeString(stream);
            CoordinatorPort = BigEndianConverter.ReadInt32(stream);
        }
    }

    struct PartitionCommitData : IMemoryStreamSerializable
    {
        public int Partition;
        public ErrorCode ErrorCode;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, Partition);
            BigEndianConverter.Write(stream, (short) ErrorCode);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Partition = BigEndianConverter.ReadInt32(stream);
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
        }
    }

    struct PartitionOffsetData : IMemoryStreamSerializable
    {
        public int Partition;
        public long Offset;
        public string Metadata;
        public ErrorCode ErrorCode;

        public void Serialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            BigEndianConverter.Write(stream, Partition);
            BigEndianConverter.Write(stream, Offset);
            Basics.SerializeString(stream, Metadata);
            BigEndianConverter.Write(stream, (short) ErrorCode);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            Partition = BigEndianConverter.ReadInt32(stream);
            Offset = BigEndianConverter.ReadInt64(stream);
            Metadata = Basics.DeserializeString(stream);
            ErrorCode = (ErrorCode) BigEndianConverter.ReadInt16(stream);
        }
    }
}
