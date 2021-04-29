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

        private static UInt256 GetMessageHash(ReadOnlyByteSpan message)
        {
            var messageHash = message.Sha256().ToHex();
            return new HashWriter().Add(MessageMagic).Add(messageHash).GetHashFinal();
        }

        public static byte[] SignMessage(this PrivateKey key, string message) => SignMessage(key, message.Utf8ToBytes());

        public static byte[] SignMessage(this PrivateKey key, ReadOnlyByteSpan message)
        {
            return key.CreateSignature(message);
        }

        public static byte[] SignMessageCompact(this PrivateKey key, UInt256 message)
        {
            return key.CreateCompactSignature(message);
        }

        public static string SignMessageToBase64(this PrivateKey key, ReadOnlyByteSpan message)
        {
            var sigBytes = key.SignMessageCompact(GetMessageHash(message));
            return sigBytes == null ? null : Convert.ToBase64String(sigBytes);
        }

        public static string SignMessageToBase64(this PrivateKey key, string message) 
            => SignMessageToBase64(key, message.Utf8ToBytes());

        public static PublicKey RecoverPubKeyFromMessage(ReadOnlyByteSpan message, ReadOnlyByteSpan signature)
            => PublicKey.FromRecoverCompact(GetMessageHash(message), signature);

        public static bool VerifyMessage(this PublicKey key, ReadOnlyByteSpan message, ReadOnlyByteSpan signature)
        {
            var rkey = RecoverPubKeyFromMessage(message, signature);
            return rkey != null && rkey == key;
        }

        public static bool VerifyMessage(this UInt160 keyId, UInt256 message, ReadOnlyByteSpan signature)
        {
            var rkey = RecoverPubKeyFromMessage(message, signature);
            return rkey != null && rkey.GetId() == keyId;
        }

        public static bool VerifyMessage(this PublicKey key, string message, string signature) 
            => VerifyMessage(key, message.Utf8ToBytes(), Convert.FromBase64String(signature));

        public static bool VerifyMessage(this UInt160 keyId, string message, string signature)
            => VerifyMessage(keyId, (UInt256)message.Utf8ToBytes(), Convert.FromBase64String(signature));
    }
}
