#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Linq;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Crypto;
using CafeLib.Bitcoin.Shared.Encoding;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Services;
using Secp256k1Net;

namespace CafeLib.Bitcoin.Shared.Keys
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

        public UInt256 GetX() => new UInt256(((ReadOnlyByteSpan) _bytes).Slice(1, 32));

        /// <summary>
        /// True if key is stored in an array of 33 bytes.
        /// False if invalid or uncompressed.
        /// </summary>
        public bool IsCompressed => _bytes?.Length == 33;

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
            if (firstByte == 2 || firstByte == 3) return 33;
            if (firstByte == 4 || firstByte == 6 || firstByte == 7) return 65;
            return 0;
        }

        public static int MinLength => 33;
        public static int MaxLength => 65;

        public ReadOnlyByteSpan ReadOnlySpan => _bytes;
        public Span<byte> Span => _bytes;

        public byte[] GetBytes() => ReadOnlySpan;

        public void Set(ReadOnlySpan<byte> data)
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

        public PublicKey()
        {
            Invalidate();
        }

        public PublicKey(bool compressed)
        {
            _bytes = new byte[compressed ? 33 : 65];
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

        public PublicKey(string hex)
            : this()
        {
            try
            {
                var vch = hex.HexToBytes();
                if ((vch.Length == 33 || vch.Length == 65) && vch.Length == PredictLength(vch[0]))
                    _bytes = vch;
            }
            catch
            {
                // ignored
            }
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
        /// The complement function is KzPrivKey's SignCompact.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="sig"></param>
        /// <returns></returns>
        public bool RecoverCompact(UInt256 hash, ReadOnlyByteSpan sig)
        {
            var (ok, vch) = Secp256K1.PublicKeyRecoverCompact(hash.Bytes, sig);

            if (!ok)
                Invalidate();
            else
                _bytes = vch;

            return ok;
        }

        public static (bool ok, PublicKey key) FromRecoverCompact(UInt256 hash, ReadOnlyByteSpan sig)
        {
            var key = new PublicKey();
            var ok = key.RecoverCompact(hash, sig);
            if (!ok) key = null;
            return (ok, key);
        }

        public bool Verify(UInt256 hash, ReadOnlySpan<byte> sig)
        {
            if (!IsValid || sig.Length == 0) return false;

            return Secp256K1.PublicKeyVerify(hash.Bytes, sig, _bytes.AsSpan());
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
        public UInt160 ToHash160() => ReadOnlySpan.Hash160();

        public string ToAddress() => Encoders.Base58Check.Encode(RootService.Network.PublicKeyAddress, ToHash160().Bytes);

        public string ToHex() => _bytes != null ? Encoders.Hex.Encode(_bytes) : "<invalid>";

        public override string ToString() => ToAddress();

        public (bool ok, PublicKey keyChild, UInt256 ccChild) Derive(uint nChild, UInt256 cc)
        {
            if (!IsValid || !IsCompressed || nChild >= HardenedBit) goto fail;

            var vout = new byte[64];
            Hashes.Bip32Hash(cc, nChild, ReadOnlySpan[0], ReadOnlySpan.Slice(1), vout);

            var sout = vout.AsSpan();
            var ccChild = new UInt256();
            sout.Slice(32, 32).CopyTo(ccChild.Bytes);

            var pkbs = new byte[64];
            if (!Secp256K1.PublicKeyParse(pkbs.AsSpan(), ReadOnlySpan)) goto fail;

            if (!Secp256K1.PubKeyTweakAdd(pkbs.AsSpan(), sout.Slice(0, 32))) goto fail;

            var dataChild = new byte[33];
            if (!Secp256K1.PublicKeySerialize(dataChild.AsSpan(), pkbs, Flags.SECP256K1_EC_COMPRESSED)) goto fail;

            var keyChild = new PublicKey(true);
            dataChild.AsSpan().CopyTo(keyChild.Span);

            return (true, keyChild, ccChild);

            fail:
            return (false, null, UInt256.Zero);
        }

        public override int GetHashCode() => _bytes.GetHashCodeOfValues();

        public bool Equals(PublicKey o) => (object) o != null && _bytes.SequenceEqual(o._bytes);
        public override bool Equals(object obj) => obj is PublicKey key && this == key;

        public static bool operator ==(PublicKey x, PublicKey y) =>
            object.ReferenceEquals(x, y) || (object) x == null && (object) y == null || x.Equals(y);

        public static bool operator !=(PublicKey x, PublicKey y) => !(x == y);
    }
}
