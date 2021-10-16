#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Threading;
using CafeLib.Core.Buffers;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;
using CafeLib.Cryptography.BouncyCastle.Crypto.Prng;
using CafeLib.Cryptography.BouncyCastle.Util;

namespace CafeLib.Cryptography
{
    public static class Randomizer
    {
        private static readonly object Mutex = new object();
        private static readonly Random Random = new Random();
        private static long _counter = Times.NanoTime();

        /// <summary>
        /// Centralized source of a cryptographic strong random entropy.
        /// </summary>
        /// <param name="entropy">Output bytes.</param>
        public static void GetStrongRandBytes(ByteSpan entropy)
        {
            GetStrongRandBytes(entropy.Length).CopyTo(entropy);
        }

        /// <summary>
        /// Centralized source of a cryptographic strong random entropy.
        /// </summary>
        /// <param name="length">How many bytes.</param>
        public static byte[] GetStrongRandBytes(int length)
        {
            var buf = new byte[length];
            var rng = new DigestRandomGenerator(new Sha256Digest());
            rng.AddSeedMaterial(NextCounterValue());
            rng.AddSeedMaterial(new Guid().ToByteArray());
            rng.NextBytes(buf);
            return buf;
        }

        /// <summary>
        /// Returns a non-cryptographic strong random number
        /// greater than or equal to zero
        /// less than one.
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            lock (Mutex)
            {
                return Random.NextDouble();
            }
        }

        /// <summary>
        /// Returns a non-cryptographically strong random integer
        /// in the range from low to high.
        /// </summary>
        /// <returns></returns>
        public static int InRange(int low, int high)
        {
            lock (Mutex)
            {
                return Random.Next(low, high);
            }
        }

        #region Helpers

        private static long NextCounterValue()
        {
            return Interlocked.Increment(ref _counter);
        }

        #endregion
    }
}