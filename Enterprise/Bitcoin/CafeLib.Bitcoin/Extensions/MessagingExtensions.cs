#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Persistence;

namespace CafeLib.Bitcoin.Extensions
{
    public static class MessagingExtensions
    {
        private const string MessageMagic = "Bitcoin Signed Message:\n";

        internal static UInt256 GetMessageHash(this ReadOnlyByteSpan message)
        {
            var messageHash = message.Sha256().ToHex();
            return new WriterHash().Add(MessageMagic).Add(messageHash).GetHashFinal();
        }
        public static byte[] SignMessage(this PrivateKey key, string message) => SignMessage(key, message.Utf8ToBytes());

        public static byte[] SignMessage(this PrivateKey key, ReadOnlyByteSpan message)
            => key.CreateCompactSignature(GetMessageHash(message));

        public static string SignMessageToBase64(this PrivateKey key, ReadOnlyByteSpan message)
        {
            var sigBytes = SignMessage(key, message);
            return sigBytes == null ? null : Convert.ToBase64String(sigBytes);
        }

        public static string SignMessageToBase64(this PrivateKey key, string message) 
            => SignMessageToBase64(key, message.Utf8ToBytes());

        public static PublicKey RecoverPubKeyFromMessage(ReadOnlySpan<byte> message, ReadOnlyByteSpan signature)
            => PublicKey.FromRecoverCompact(GetMessageHash(message), signature);

        public static bool VerifyMessage(this PublicKey key, ReadOnlySpan<byte> message, ReadOnlyByteSpan signature)
        {
            var rkey = RecoverPubKeyFromMessage(message, signature);
            return rkey != null && rkey == key;
        }

        public static bool VerifyMessage(this UInt160 keyId, ReadOnlySpan<byte> message, ReadOnlyByteSpan signature)
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
