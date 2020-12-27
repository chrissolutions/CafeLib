﻿// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using CafeLib.KafkaSharp.Common;

namespace CafeLib.KafkaSharp.Cluster
{
    /// <summary>
    /// A utility class used to configure and manage internally used object pools.
    /// </summary>
    class Pools
    {
        private readonly IStatistics _stats;
        private readonly ILogger _logger;

        public Pools(IStatistics stats, ILogger logger)
        {
            _stats = stats;
            _logger = logger;
        }

        public Pool<byte[]> SocketBuffersPool { get; private set; }

        public void InitSocketBuffersPool(int buffersLength)
        {
            SocketBuffersPool = new Pool<byte[]>(
                () =>
                {
                    _stats.UpdateSocketBuffers(1);
                    return new byte[buffersLength];
                }, (s, b) => { });
        }

        public Pool<ReusableMemoryStream> MessageBuffersPool { get; private set; }

        public void InitMessageBuffersPool(int limit, int maxChunkSize)
        {
            MessageBuffersPool = new Pool<ReusableMemoryStream>(
                limit,
                () =>
                {
                    _stats.UpdateMessageBuffers(1);
                    return new ReusableMemoryStream(MessageBuffersPool, _logger);
                },
                (b, reused) =>
                {
                    if (!reused)
                    {
                        _stats.UpdateMessageBuffers(-1);
                    }
                    else
                    {
                        b.SetLength(0);
                        if (b.Capacity > maxChunkSize)
                        {
                            b.Capacity = maxChunkSize;
                        }
                    }
                });
        }

        public Pool<ReusableMemoryStream> RequestsBuffersPool { get; private set; }

        public void InitRequestsBuffersPool()
        {
            RequestsBuffersPool = new Pool<ReusableMemoryStream>(
                () =>
                {
                    _stats.UpdateRequestsBuffers(1);
                    return new ReusableMemoryStream(RequestsBuffersPool, _logger);
                },
                (b, reused) =>
                {
                    if (!reused)
                    {
                        _stats.UpdateRequestsBuffers(-1);
                    }
                    else
                    {
                        b.SetLength(0);
                    }
                });
        }
    }
}