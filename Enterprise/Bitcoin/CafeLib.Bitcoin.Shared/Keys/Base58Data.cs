#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Shared.Encoding;

namespace CafeLib.Bitcoin.Shared.Keys
{
    /// <summary>
    /// Base class for Base58 encoded objects.
    /// </summary>
    public class Base58Data : IComparable<Base58Data>
    {
        protected byte[] _VersionData;
        protected int _VersionLength;

        protected Span<byte> _Version => new Span<byte>(_VersionData, 0, _VersionLength);
        protected Span<byte> _Data => new Span<byte>(_VersionData, _VersionLength, _VersionData.Length - _VersionLength);
        protected ReadOnlySpan<byte> VersionData => _VersionData.AsSpan();
        protected ReadOnlySpan<byte> Version => _Version;
        protected ReadOnlySpan<byte> Data => _Data;

        protected Base58Data()
        {
        }

        protected void SetData(byte[] versionData, int versionLength = 1)
        {
            _VersionData = versionData;
            _VersionLength = versionLength;
        }

        protected void SetData(ReadOnlySpan<byte> version, ReadOnlySpan<byte> data, bool flag)
        {
            _VersionData = new byte[version.Length + data.Length + 1];
            _VersionLength = version.Length;
            version.CopyTo(_Version);
            data.CopyTo(_Data);
            _Data[^1] = (byte)(flag ? 1 : 0);
        }

        protected void SetData(ReadOnlySpan<byte> version, ReadOnlySpan<byte> data)
        {
            _VersionData = new byte[version.Length + data.Length];
            _VersionLength = version.Length;
            version.CopyTo(_Version);
            data.CopyTo(_Data);
        }

        protected bool SetString(string b58, int nVersionBytes)
        {
            if (!Encoders.Base58Check.TryDecode(b58, out var bytes) || bytes.Length < nVersionBytes)
            {
                _VersionData = new byte[0];
                _VersionLength = 0;
                return false;
            }

            _VersionData = bytes;
            _VersionLength = nVersionBytes;
            return true;
        }

        public override string ToString() => Encoders .Base58Check.Encode(_VersionData);

        public override int GetHashCode() => ToString().GetHashCode();
        public bool Equals(Base58Data o) => (object)o != null && Enumerable.SequenceEqual(_VersionData, o._VersionData);
        public override bool Equals(object obj) => obj is Base58Data && this == (Base58Data)obj;
        public static bool operator ==(Base58Data x, Base58Data y) => object.ReferenceEquals(x, y) || (object)x == null && (object)y == null || x.Equals(y);
        public static bool operator !=(Base58Data x, Base58Data y) => !(x == y);

        public int CompareTo(Base58Data o) => o == null ? 1 : VersionData.SequenceCompareTo(o.VersionData);
        public static bool operator <(Base58Data a, Base58Data b) => a.CompareTo(b) < 0;
        public static bool operator >(Base58Data a, Base58Data b) => a.CompareTo(b) > 0;
        public static bool operator <=(Base58Data a, Base58Data b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Base58Data a, Base58Data b) => a.CompareTo(b) >= 0;
    }
}
