#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Buffers;

namespace CafeLib.Bitcoin.Keys
{
    public static class Randomizer
    {
        private static readonly object Mutex = new object();
        private static readonly Random Random = new Random();

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
        public static Span<byte> GetStrongRandBytes(int length)
        {
            var buf = new byte[length];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buf);
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
    }
}
