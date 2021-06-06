#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Encoding;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Services;
using Secp256k1Net;

namespace CafeLib.Bitcoin.Keys
{
    /// <summary>
    /// A KzPubKey is fundamentally an array of bytes in one of these states:
    /// null: Key is invalid.
    /// byte[33]: Key is compressed.
    /// byte[65]: Key is uncompressed.
    /// </summary>
    public class PublicKey
    {
        /// <summary>
        /// A KzPubKey is fundamentally an array of bytes in one of these states:
        /// null: Key is invalid.
        /// byte[33]: Key is compressed.
        /// byte[65]: Key is uncompressed.
        /// </summary>
        private byte[] _bytes;

        private readonly Guid _hashCode;

        /// <summary>
        /// 
        /// </summary>
        private static readonly Lazy<Secp256k1> LazySecp256K1 = new Lazy<Secp256k1>(() =>
        {
            var ctx = new Secp256k1();
            ctx.Randomize(Randomizer.GetStrongRandBytes(32));
            return ctx;
        }, true);

        private const uint HardenedBit = 0x80000000;

        private static Secp256k1 Secp256K1 => LazySecp256K1.Value;

        internal const int CompressedLength = 33;
        internal const int UncompressedLength = 65;

        public PublicKey()
        {
            _hashCode = new Guid();
            Invalidate();
        }

        public PublicKey(bool compressed)
            : this()
        {
            _bytes = new byte[compressed ? CompressedLength : UncompressedLength];
        }

        public PublicKey(ReadOnlyByteSpan bytes)
            : this()
        {
            if (bytes.Length > 0 && bytes.Length == PredictLength(bytes[0]))
            {
                _bytes = new byte[bytes.Length];
                bytes.CopyTo(_bytes);
            }
        }

        public PublicKey(VarType varType)
            : this()
        {
            var firstByte = varType.FirstByte;
            var size = PredictLength(firstByte);

            _bytes = new byte[size]; 
            varType.CopyTo(_bytes);
        }

        public PublicKey(string hex)
            : this()
        {
            try
            {
                var vch = hex.HexToBytes();
                if ((vch.Length == CompressedLength || vch.Length == UncompressedLength) && vch.Length == PredictLength(vch[0]))
                    _bytes = vch;
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Creates a copy of this key.
        /// </summary>
        /// <returns></returns>
        public PublicKey Clone()
        {
            var clone = new PublicKey();
            if (_bytes != null)
                clone._bytes = _bytes.ToArray();
            return clone;
        }

        /// <summary>
        /// True if key is stored in an array of 33 bytes.
        /// False if invalid or uncompressed.
        /// </summary>
        public bool IsCompressed => _bytes?.Length == CompressedLength;

        /// <summary>
        /// True if key is defined and either compressed or uncompressed.
        /// False if array of bytes is null.
        /// </summary>
        public bool IsValid => _bytes != null;

        /// <summary>
        /// Compute the length of a pubkey with a given first byte.
        /// </summary>
        /// <param name="firstByte">First byte of KzPubKey Span.</param>
        /// <returns>0, 33, or 65</returns>
        private static int PredictLength(byte firstByte)
        {
            if (firstByte == 2 || firstByte == 3) return CompressedLength;
            if (firstByte == 4 || firstByte == 6 || firstByte == 7) return UncompressedLength;
            return 0;
        }

        public ReadOnlyByteSpan Data => _bytes;

        public byte[] ToArray() => Data;

        public void Set(ReadOnlyByteSpan data)
        {
            var len = data.Length == 0 ? 0 : PredictLength(data[0]);
            if (len > 0 && len == data.Length)
            {
                _bytes = new byte[data.Length];
                data.CopyTo(_bytes.AsSpan());
            }
            else
                Invalidate();
        }

        private void Invalidate()
        {
            _bytes = null;
        }

        /// <summary>
        /// Check whether a signature is normalized (lower-S).
        /// </summary>
        /// <param name="vchSig"></param>
        /// <returns></returns>
        public static bool CheckLowS(ReadOnlyByteSequence vchSig)
        {
            var sig = new byte[64];
            var input = (ReadOnlyByteSpan)vchSig;

            using var library = new Secp256k1();
            if (!library.SignatureParseDerLax(sig, input)) return false;
            return !library.SignatureNormalize(Span<byte>.Empty, input);
        }

        /// <summary>
        /// The complement function is PrivateKey CreateCompatSignature.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        public bool RecoverCompact(UInt256 hash, ReadOnlyByteSpan sig)
        {
            var (ok, vch) = Secp256K1.PublicKeyRecoverCompact(hash.Span, sig);

            if (!ok)
                Invalidate();
            else
                _bytes = vch;

            return ok;
        }

        public static PublicKey FromRecoverCompact(UInt256 hash, ReadOnlyByteSpan sig)
        {
            var key = new PublicKey();
            return key.RecoverCompact(hash, sig) ? key : null;
        }

        public bool Verify(UInt256 hash, VarType sig)
        {
            if (!IsValid || sig.Length == 0) return false;
            return Secp256K1.PublicKeyVerify(hash.Span, (ReadOnlyByteSpan)sig, Data);
        }

        /// <summary>
        /// RIPEMD160 applied to SHA256 of the 33 or 65 public key bytes.
        /// </summary>
        /// <returns>20 byte hash as a KzUInt160</returns>
        public UInt160 GetId() => ToHash160();

        /// <summary>
        /// RIPEMD160 applied to SHA256 of the 33 or 65 public key bytes.
        /// </summary>
        /// <returns>20 byte hash as a KzUInt160</returns>
        public UInt160 ToHash160() => Data.Hash160();

        public string ToAddress() => Encoders.Base58Check.Encode(RootService.Network.PublicKeyAddress, ToHash160().Span);

        public string ToHex() => _bytes != null ? Encoders.Hex.Encode(_bytes) : "<invalid>";

        public override string ToString() => ToAddress();

        public (bool ok, PublicKey keyChild, UInt256 ccChild) Derive(uint nChild, UInt256 cc)
        {
            (bool ok, PublicKey keyChild, UInt256 ccChild) invalid = (false, null, UInt256.Zero);
            if (!IsValid || !IsCompressed || nChild >= HardenedBit) return invalid;

            var vout = new byte[64];
            Hashes.Bip32Hash(cc, nChild, Data[0], Data[1..], vout);

            var ccChild = new UInt256();
            vout[UInt256.Length..].CopyTo(ref ccChild);

            var pkBytes = new byte[64];
            if (!Secp256K1.PublicKeyParse(pkBytes.AsSpan(), Data)) return invalid;
            if (!Secp256K1.PubKeyTweakAdd(pkBytes, vout[..UInt256.Length])) return invalid;

            var dataChild = new byte[CompressedLength];
            if (!Secp256K1.PublicKeySerialize(dataChild.AsSpan(), pkBytes, Flags.SECP256K1_EC_COMPRESSED)) return invalid;

            var keyChild = new PublicKey(true);
            dataChild.CopyTo(ref keyChild._bytes);

            return (true, keyChild, ccChild);
        }

        public override int GetHashCode() => _hashCode.GetHashCode();

        public bool Equals(PublicKey o) => !(o is null) && _bytes.SequenceEqual(o._bytes);
        public override bool Equals(object obj) => obj is PublicKey key && this == key;

        public static explicit operator UInt256(PublicKey rhs) => new UInt256(rhs._bytes.Slice(1, UInt256.Length));

        public static explicit operator PublicKey(byte[] rhs) => new PublicKey(rhs);
        public static implicit operator byte[](PublicKey rhs) => rhs._bytes;

        public static implicit operator ByteSpan(PublicKey rhs) => rhs._bytes.AsSpan();
        public static implicit operator PublicKey(ByteSpan rhs) => new PublicKey(rhs);

        public static implicit operator ReadOnlyByteSpan(PublicKey rhs) => rhs._bytes.AsSpan();
        public static implicit operator PublicKey(ReadOnlyByteSpan rhs) => new PublicKey(rhs);

        public static bool operator ==(PublicKey x, PublicKey y) => x?.Equals(y) ?? y is null;
        public static bool operator !=(PublicKey x, PublicKey y) => !(x == y);
    }
}
