﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Utility
{
    public class KzRangeIndexTests
    {
        /// <summary>
        /// Verify C# Core 3.0 Index and Range features are working...
        /// </summary>
        [Fact]
        public void SpanIndexing()
        {
            var d = new [] { 1, 2, 3, 4 }.AsSpan();
            Assert.Equal(new[] { 2 }, d[1..2].ToArray());
            Assert.Equal(new[] { 2, 3 }, d[1..3].ToArray());
            Assert.Equal(new[] { 1, 2, 3, 4 }, d[0..4].ToArray());
            Assert.Equal(new[] { 2 }, d[1..^2].ToArray());
            Assert.Equal(new[] { 2, 3 }, d[1..^1].ToArray());
            Assert.Equal(new[] { 1, 2, 3, 4 }, d[0..^0].ToArray());
            Assert.Equal(4, d[^1]);
            Assert.Equal(3, d[^2]);
        }
    }
}
