﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    using Serializers = Tuple<ISerializer, ISerializer>;

    class ProduceRequest : ISerializableRequest
    {
        public IEnumerable<TopicData<PartitionData>> TopicsData;
        public int Timeout;
        public short RequiredAcks;
        public string TransactionalID;

        #region Serialization

        public ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId, object extra, Basics.ApiVersion version)
        {
            return CommonRequest.Serialize(target, this, correlationId, clientId, Basics.ApiKey.ProduceRequest, version, extra);
        }

        public void SerializeBody(ReusableMemoryStream stream, object extra, Basics.ApiVersion version)
        {
            if (version >= Basics.ApiVersion.V3)
            {
                Basics.SerializeString(stream, TransactionalID);
            }
            BigEndianConverter.Write(stream, RequiredAcks);
            BigEndianConverter.Write(stream, Timeout);
            Basics.WriteArray(stream, TopicsData, extra, version);
        }

        #endregion
    }

    class PartitionData : IMemoryStreamSerializable
    {
        public IEnumerable<Message> Messages;
        public int Partition;
        public CompressionCodec CompressionCodec;

        #region Serialization

        struct SerializationInfo
        {
            public Serializers Serializers;
            public CompressionCodec CompressionCodec;
            public MessageVersion MessageVersion;
        }

        public void Serialize(ReusableMemoryStream stream, object extra, Basics.ApiVersion version)
        {
            BigEndianConverter.Write(stream, Partition);

            if (version >= Basics.ApiVersion.V3)
            {
                ISerializer keySerializer = null, valueSerializer = null;
                if (extra is Serializers asSerializers)
                {
                    keySerializer = asSerializers.Item1;
                    valueSerializer = asSerializers.Item2;
                }

                SerializeRecordBatch(stream, keySerializer, valueSerializer);
            }
            else
            {
                SerializeMessageSet(stream, extra as Serializers, version >= Basics.ApiVersion.V2 ? MessageVersion.V1 : MessageVersion.V0);
            }
        }

        private void SerializeRecordBatch(ReusableMemoryStream stream, ISerializer keySerializer,
            ISerializer valueSerializer)
        {
            // Starting Produce request V3, messages are encoded in the new RecordBatch.
            var batch = new RecordBatch
            {
                CompressionCodec = CompressionCodec,
                Records = Messages.Select(message =>
                {
                    SerializeMessageIfNotSized(ref message, keySerializer, valueSerializer, stream.Pool);
                    return new Record
                    {
                        Key = message.Key,
                        Value = message.Value,
                        Headers = message.Headers,
                        Timestamp = message.TimeStamp,
                        // If the serializer is not compatible, we already resolved this
                        // previously, so it's ok if the cast returns null
                        KeySerializer = keySerializer as ISizableSerializer,
                        ValueSerializer = valueSerializer as ISizableSerializer,
                        SerializedKeyValue = message.SerializedKeyValue,
                    };
                }),
            };

            Basics.WriteWithSize(stream, batch.Serialize);
        }

        private static void SerializeMessageIfNotSized(ref Message msg, ISerializer keySerializer, ISerializer valueSerializer, Pool<ReusableMemoryStream> pool)
        {
            if (msg.SerializedKeyValue == null
                && (Basics.SizeOfSerializedObject(msg.Key, keySerializer as ISizableSerializer) == Basics.UnknownSize
                    || Basics.SizeOfSerializedObject(msg.Value, valueSerializer as ISizableSerializer) == Basics.UnknownSize))
            {
                msg.SerializeKeyValue(pool.Reserve(), new Serializers(keySerializer, valueSerializer), Compatibility.V0_11_0);
            }
        }

        private void SerializeMessageSet(ReusableMemoryStream stream, Serializers serializers, MessageVersion version)
        {
            Basics.WriteWithSize(stream, Messages,
                new SerializationInfo
                {
                    Serializers = serializers,
                    CompressionCodec = CompressionCodec,
                    MessageVersion = version
                }, SerializeMessages);
        }

        private static void SerializeMessagesUncompressed(ReusableMemoryStream stream, IEnumerable<Message> messages,
            Serializers serializers, MessageVersion msgVersion)
        {
            long offset = 0;
            foreach (var message in messages)
            {
                // We always set offsets starting from 0 and increasing by one for each consecutive message.
                // This is because in compressed messages, when message format is V1, the brokers
                // will follow this format on disk. You're expected to do the same if you want to
                // avoid offset reassignment and message recompression.
                // When message format is V0, brokers will rewrite the offsets anyway
                // so we use the same scheme in all cases.
                BigEndianConverter.Write(stream, offset++);
                Basics.WriteWithSize(stream, message,
                    new SerializationInfo
                    {
                        CompressionCodec = CompressionCodec.None,
                        Serializers = serializers,
                        MessageVersion = msgVersion
                    }, SerializeMessageWithCodec);
            }
        }

        // Dumb trick to minimize closure allocations
        private static readonly Action<ReusableMemoryStream, IEnumerable<Message>, SerializationInfo> SerializeMessages =
            _SerializeMessages;

        // Dumb trick to minimize closure allocations
        private static readonly Action<ReusableMemoryStream, Message, SerializationInfo> SerializeMessageWithCodec =
            _SerializeMessageWithCodec;

        private static void _SerializeMessages(ReusableMemoryStream stream, IEnumerable<Message> messages, SerializationInfo info)
        {
            if (info.CompressionCodec != CompressionCodec.None)
            {
                stream.Write(Basics.Zero64, 0, 8);
                using (var msgsetStream = stream.Pool.Reserve())
                {
                    SerializeMessagesUncompressed(msgsetStream, messages, info.Serializers, info.MessageVersion);

                    using (var compressed = stream.Pool.Reserve())
                    {
                        Basics.CompressStream(msgsetStream, compressed, info.CompressionCodec);

                        var m = new Message
                        {
                            Value = compressed,
                            TimeStamp = Timestamp.Now
                        };
                        Basics.WriteWithSize(stream, m,
                            new SerializationInfo
                            {
                                Serializers = SerializationConfig.ByteArraySerializers,
                                CompressionCodec = info.CompressionCodec,
                                MessageVersion = info.MessageVersion
                            }, SerializeMessageWithCodec);
                    }
                }
            }
            else
            {
                SerializeMessagesUncompressed(stream, messages, info.Serializers, info.MessageVersion);
            }
        }

        private static void _SerializeMessageWithCodec(ReusableMemoryStream stream, Message message, SerializationInfo info)
        {
            message.Serialize(stream, info.CompressionCodec, info.Serializers, info.MessageVersion);
        }

        public void Deserialize(ReusableMemoryStream stream, object _, Basics.ApiVersion __)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

