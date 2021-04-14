﻿using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.Bitcoin.Shared.Buffers;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class ByteExtensions
    {
        public static int AggregateHashCode(this IEnumerable<byte> bytes) => bytes?.Aggregate(17, (current, b) => current * 31 + b) ?? 0;

        public static Span<byte> Slice(this byte[] a, int start) => a.AsSpan().Slice(start);
        public static ByteSpan Slice(this byte[] a, int start, int length) => a.AsSpan().Slice(start, length);
    }
}