#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeLib.Bitcoin.Keys
{

    /// <summary>
    /// Represent a BIP32 style key path.
    /// </summary>
    public class KeyPath
	{
        /// <summary>
        /// Creates an empty path (zero indices) with FromPriv set to null.
        /// </summary>
        public KeyPath()
        {
            FromPrivateKey = null;
            Indices = new UInt32[0];
        }

        /// <summary>
        /// True if the path starts with m.
        /// False if the path starts with M.
        /// null if the path starts with an index.
        /// </summary>
        public bool? FromPrivateKey { get; }

        /// <summary>
        /// Path indices, in order.
        /// Hardened indices have the 0x80000000u bit set.
        /// </summary>
        public UInt32[] Indices { get; }

		public UInt32 this[int index] => Indices[index];

        /// <summary>
        /// How many numeric Indices there are.
        /// </summary>
        public int Count => Indices.Length;

        /// <summary>
        /// HardenedBit is 0x80000000u.
        /// </summary>
        public const UInt32 HardenedBit = 0x80000000u;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
		private static UInt32 ParseIndex(string i)
		{
			var hardened = i.Length > 0 && i[^1] == '\'' || i[^1] == 'H';
			var index = UInt32.Parse(hardened ? i[..^1] : i);
            if (index >= HardenedBit)
                throw new ArgumentException($"Indices must be less than {HardenedBit}.");
			return hardened ? index | HardenedBit : index;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static UInt32[] ParseIndices(string path)
        {
			return path.Split('/').Where(p => p != "m" && p != "M" && p != "").Select(ParseIndex).ToArray();
        }

        /// <summary>
        /// Returns a sequence of KzKeyPaths from comma separated string of paths.
        /// </summary>
        /// <param name="v">Comma separated string of paths.</param>
        /// <returns></returns>
        public static IEnumerable<KeyPath> AsEnumerable(string v)
        {
            return v.Split(',').Select(kp => new KeyPath(kp));
        }

        /// <summary>
        /// Parse a KzHDKeyPath
        /// </summary>
        /// <param name="path">The KzHDKeyPath formated like a/b/c'/d. Appostrophe indicates hardened/private. a,b,c,d must convert to 0..2^31.
        /// Optionally the path can start with "m/" for private extended master key derivations or "M/" for public extended master key derivations.
        /// </param>
        /// <returns></returns>
        public static KeyPath Parse(string path)
		{
			return new KeyPath(path);
		}

		/// <summary>
		/// Creates a path based on its formatted string representation.
		/// </summary>
		/// <param name="path">The KzHDKeyPath formated like a/b/c'/d. Appostrophe indicates hardened/private. a,b,c,d must convert to 0..2^31.
        /// Optionally the path can start with "m/" for private extended master key derivations or "M/" for public extended master key derivations.
        /// </param>
		/// <returns></returns>
		public KeyPath(string path)
		{
            FromPrivateKey = path.StartsWith('m') ? true : path.StartsWith('M') ? false : (bool?)null;
            Indices = ParseIndices(path);
		}

        /// <summary>
        /// Creates a path with the properties provided.
        /// FromPriv is set to null.
        /// </summary>
        /// <param name="indices">Sets the indices. Hardened indices must have the HardenedBit set.</param>
		public KeyPath(params UInt32[] indices)
		{
            FromPrivateKey = null;
			Indices = indices;
		}

        /// <summary>
        /// Creates a path with the properties provided.
        /// </summary>
        /// <param name="fromPrivate">Sets FromPriv if provided.</param>
        /// <param name="indices">Sets the indices. Hardened indices must have the HardenedBit set.</param>
		public KeyPath(bool? fromPrivate, params UInt32[] indices)
		{
            FromPrivateKey = fromPrivate;
			Indices = indices;
		}

        /// <summary>
        /// Extends path with additional indices.
        /// FromPrivateKey of additionalIndices is ignored.
        /// </summary>
        /// <param name="additionalIndices"></param>
        /// <returns>New path with concatenated indices.</returns>
		public KeyPath Derive(KeyPath additionalIndices)
		{
			return new KeyPath(FromPrivateKey, Indices.Concat(additionalIndices.Indices).ToArray());
		}

        /// <summary>
        /// Extends path with additional index.
        /// </summary>
        /// <param name="index">Values with HardenedBit set are hardened.</param>
        /// <returns>New path with concatenated index.</returns>
		public KeyPath Derive(UInt32 index)
		{
            return new KeyPath(FromPrivateKey, Indices.Concat(new[] { index }).ToArray());
		}

        /// <summary>
        /// Extends path with additional index.
        /// </summary>
        /// <param name="index">Value must be non-negative and less than HardenedBit (which an int always is...)</param>
        /// <param name="hardened">If true, HardenedBit will be added to index.</param>
        /// <returns>New path with concatenated index.</returns>
		public KeyPath Derive(int index, bool hardened)
		{
			if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Must be non-negative.");
			var i = (UInt32)index;
            return Derive(hardened ? i | HardenedBit : i);
		}

        /// <summary>
        /// Extends path with additional indices from string formatted path.
        /// Any "m/" or "M/" prefix in path will be ignored.
        /// </summary>
        /// <param name="path">The indices in path will be concatenated.</param>
        /// <returns>New path with concatenated indices.</returns>
		public KeyPath Derive(string path)
		{
			return Derive(new KeyPath(path));
		}

        /// <summary>
        /// Returns a new path with one less index, or null if path has no indices.
        /// </summary>
		public KeyPath Parent => Count == 0 ? null : new KeyPath(FromPrivateKey, Indices.Take(Indices.Length - 1).ToArray());

        /// <summary>
        /// Returns a new path with the last index incremented by one.
        /// Throws InvalidOperation if path contains no indices.
        /// </summary>
        /// <returns>Returns a new path with the last index incremented by one.</returns>
        public KeyPath Increment()
        {
            if (Count == 0) throw new InvalidOperationException();
            var indices = Indices.ToArray();
            indices[Count - 1]++;
            return new KeyPath(FromPrivateKey, indices);
        }

		public override string ToString()
		{
            var sb = new StringBuilder();
            sb.Append(FromPrivateKey != null && FromPrivateKey.Value ? "m/" : "M/");

            foreach (var i in Indices) 
            {
                sb.Append(i & ~HardenedBit);
                if (i >= HardenedBit) sb.Append("'");
                sb.Append("/");
            }

            sb.Length--;
            return sb.ToString();
		}

		public override int GetHashCode() => ToString().GetHashCode();
        public bool Equals(KeyPath o) => (object)o != null && ToString().Equals(o.ToString());
        public override bool Equals(object obj) => obj is KeyPath path && this == path;
        public static bool operator ==(KeyPath x, KeyPath y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(KeyPath x, KeyPath y) => !(x == y);

        /// <summary>
        /// Returns true if HardenedBit is set on last index.
        /// Throws InvalidOperation if there are no indices.
        /// </summary>
		public bool IsHardened
		{
			get
            {
                if (Count == 0) throw new InvalidOperationException("No index found in this KzHDKeyPath");
                return (Indices[Count - 1] & HardenedBit) != 0;
            }
        }

        public static implicit operator KeyPath(string s) => new KeyPath(s);
    }
}
