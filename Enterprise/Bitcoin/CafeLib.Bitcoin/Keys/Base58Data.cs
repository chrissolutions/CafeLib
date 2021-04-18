#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Encoding;

namespace CafeLib.Bitcoin.Keys
{
    /// <summary>
    /// Base class for Base58 encoded objects.
    /// </summary>
    public class Base58Data : IComparable<Base58Data>
    {
        private byte[] _versionData;
        private int _versionLength;

        protected ByteSpan Version => new Span<byte>(_versionData, 0, _versionLength);
        protected ByteSpan Data => new Span<byte>(_versionData, _versionLength, _versionData.Length - _versionLength);
        protected ReadOnlyByteSpan VersionData => _versionData.AsSpan();
        //protected ReadOnlySpan<byte> Version => _Version;
        //protected ReadOnlySpan<byte> Data => _Data;

        protected Base58Data()
        {
        }

        protected void SetData(byte[] versionData, int versionLength = 1)
        {
            _versionData = versionData;
            _versionLength = versionLength;
        }

        protected void SetData(ReadOnlySpan<byte> version, ReadOnlySpan<byte> data, bool flag)
        {
            _versionData = new byte[version.Length + data.Length + 1];
            _versionLength = version.Length;
            version.CopyTo(Version);
            data.CopyTo(Data);
            Data.Data[^1] = (byte)(flag ? 1 : 0);
        }

        protected void SetData(ReadOnlySpan<byte> version, ReadOnlySpan<byte> data)
        {
            _versionData = new byte[version.Length + data.Length];
            _versionLength = version.Length;
            version.CopyTo(Version);
            data.CopyTo(Data);
        }

        protected bool SetString(string b58, int nVersionBytes)
        {
            if (!Encoders.Base58Check.TryDecode(b58, out var bytes) || bytes.Length < nVersionBytes)
            {
                _versionData = new byte[0];
                _versionLength = 0;
                return false;
            }

            _versionData = bytes;
            _versionLength = nVersionBytes;
            return true;
        }

        public override string ToString() => Encoders .Base58Check.Encode(_versionData);

        public override int GetHashCode() => ToString().GetHashCode();
        public bool Equals(Base58Data o) => (object)o != null && Enumerable.SequenceEqual(_versionData, o._versionData);
        public override bool Equals(object obj) => obj is Base58Data base58Data && this == base58Data;
        public static bool operator ==(Base58Data x, Base58Data y) => ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(Base58Data x, Base58Data y) => !(x == y);

        public int CompareTo(Base58Data o) => o == null ? 1 : VersionData.Data.SequenceCompareTo(o.VersionData);
        public static bool operator <(Base58Data a, Base58Data b) => a.CompareTo(b) < 0;
        public static bool operator >(Base58Data a, Base58Data b) => a.CompareTo(b) > 0;
        public static bool operator <=(Base58Data a, Base58Data b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Base58Data a, Base58Data b) => a.CompareTo(b) >= 0;
    }
}
