﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Protocol
{
    // Convenience interface to avoid code duplication.
    // quite ugly from OO perspective but struct cannot inherit in C#.
    interface ISerializableRequest
    {
        ReusableMemoryStream Serialize(ReusableMemoryStream target, int correlationId, byte[] clientId, object extra, Basics.ApiVersion version);
        void SerializeBody(ReusableMemoryStream stream, object extra, Basics.ApiVersion version);
    }

    // Convenience class to avoid code duplication. We cannot
    // use inheritance with structs so we resort to this dumb trick
    // and ugly ISerializableRequest.
    static class CommonRequest
    {
        public static ReusableMemoryStream Serialize<TRequest>(ReusableMemoryStream stream, TRequest request, int correlationId, byte[] clientId,
            Basics.ApiKey apiKey, Basics.ApiVersion apiVersion, object extra) where TRequest : ISerializableRequest
        {
            Basics.WriteRequestHeader(stream, correlationId, apiKey, apiVersion, clientId);
            request.SerializeBody(stream, extra, apiVersion);
            Basics.WriteMessageLength(stream);
            stream.Position = 0;
            return stream;
        }
    }
}