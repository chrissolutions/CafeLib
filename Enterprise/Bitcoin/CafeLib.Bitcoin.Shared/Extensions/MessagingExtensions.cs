#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Crypto;
using CafeLib.Bitcoin.Shared.Keys;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Persistence;

namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class MessagingExtensions
    {
        private const string MessageMagic = "Bitcoin Signed Message:\n";

        internal static UInt256 GetMessageHash(this ReadOnlyByteSpan message)
        {
            var messageHash = message.Sha256().ToHex();
            return new WriterHash().Add(MessageMagic).Add(messageHash).GetHashFinal();
        }

        public static byte[] SignMessage(this PrivateKey key, ReadOnlySpan<byte> message)
        {
            var (ok, sig) = key.CreateCompactSignature(GetMessageHash(message));
            return ok ? sig : null;
        }

        public static string SignMessageToB64(this PrivateKey key, ReadOnlySpan<byte> message)
        {
            var sigBytes = SignMessage(key, message);
            return sigBytes == null ? null : Convert.ToBase64String(sigBytes);
        }

        public static byte[] SignMessage(this PrivateKey key, string message) => SignMessage(key, message.Utf8ToBytes());

        public static string SignMessageToBase64(this PrivateKey key, string message) => SignMessageToB64(key, message.Utf8ToBytes());

        public static PublicKey RecoverPubKeyFromMessage(ReadOnlySpan<byte> message, ReadOnlyByteSpan signature)
        {
            return PublicKey.FromRecoverCompact(GetMessageHash(message), signature);
        }

        public static bool VerifyMessage(this PublicKey key, ReadOnlySpan<byte> message, ReadOnlySpan<byte> signature)
        {
            var rkey = RecoverPubKeyFromMessage(message, signature);
            return rkey != null && rkey == key;
        }

        public static bool VerifyMessage(this UInt160 keyId, ReadOnlySpan<byte> message, ReadOnlySpan<byte> signature)
        {
            var rkey = RecoverPubKeyFromMessage(message, signature);
            return rkey != null && rkey.GetId() == keyId;
        }

        public static bool VerifyMessage(this PublicKey key, string message, string signature) 
            => VerifyMessage(key, message.Utf8ToBytes(), Convert.FromBase64String(signature));

        public static bool VerifyMessage(this UInt160 keyId, string message, string signature)
            => VerifyMessage(keyId, message.Utf8ToBytes(), Convert.FromBase64String(signature));
    }
}
