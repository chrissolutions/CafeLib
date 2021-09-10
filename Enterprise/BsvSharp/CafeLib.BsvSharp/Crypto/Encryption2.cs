using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CafeLib.BsvSharp.BouncyCastle.Crypto;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Digests;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Engines;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Paddings;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Parameters;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Prng;
using CafeLib.BsvSharp.BouncyCastle.Security;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;
using CafeLib.Core.Buffers;
using CafeLib.Core.Support;
// ReSharper disable InconsistentNaming

namespace CafeLib.BsvSharp.Crypto
{
    public static class Encryption2 
    {
        private const string Algorithm = "AES";

        private const int DefaultVectorLength = 16;
        private const int DefaultKeyLength = 16;
        private const int DefaultSaltLength = 8;
        private const int DefaultIterations = 2048;

        private const byte AesIvSize = 16;

        private static readonly string AesCryptoService = $"{Algorithm}/{AesTypes.CipherMode.CBC}/{AesTypes.Padding.PKCS7}";

        public static byte[] InitializationVector(ReadOnlyByteSpan key, ReadOnlySpan<byte> data, int length = DefaultVectorLength)
            => key.HmacSha256(data).Span.Slice(0, length).ToArray();

        public static byte[] KeyFromPassword(NonNullable<string> password, byte[] salt = null, int iterations = DefaultIterations, int keyLength = DefaultKeyLength)
            => KeyFromPassword(password.Value.Utf8ToBytes(), salt, iterations, keyLength);

        public static byte[] KeyFromPassword(NonNullable<byte[]> password, byte[] salt = null, int iterations = DefaultIterations, int keyLength = DefaultKeyLength)
        {
            salt ??= SaltBytes();
            var derive = new Pkcs5S2DeriveBytes();
            return derive.GenerateDerivedKey(keyLength, password.Value, salt, iterations);
        }

        /// <summary>
        /// Applies AES encryption to data with the specified key.
        /// iv can be specified (must be 16 bytes) or left null.
        /// The actual iv used is added to the first 16 bytes of the output result unless noIV = true.
        /// Padding is PKCS7.
        /// Mode is CBC.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="key">The encryption key to use. The same key must be used to decrypt.</param>
        /// <param name="iv">null or 16 bytes of random initialization vector data</param>
        /// <param name="noInitVector">If true, the initialization vector used is not prepended to the encrypted result.</param>
        /// <returns>16 bytes of IV followed by encrypted data bytes.</returns>
        public static byte[] AesEncrypt(ReadOnlyByteSpan data, byte[] key, byte[] iv = null, bool noInitVector = false)
        {
            iv ??= GenerateIV();
            var aes = CreateAes(key, iv);
            var aesData = aes.DoFinal(data);
            return noInitVector ? aesData : iv.Concat(aesData).ToArray();
        }

        /// <summary>
        /// Encrypted message
        /// </summary>
        /// <param name="message">message to be encrypted</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public static byte[] AesEncrypt(string message, string password)
        {
            var bytes = message.Utf8ToBytes();
            var keySalt = SaltBytes();
            var key = KeyFromPassword(password, keySalt);
            var iv = InitializationVector(key, bytes);
            var data = AesEncrypt(bytes, key, iv, true);

            return PackArrays(keySalt, key, iv, data);
        }

        /// <summary>
        /// Applies AES decryption to data encrypted with AesEncrypt and the specified key.
        /// The actual iv used is obtained from the first 16 bytes of data if null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv">The IV to use. If null, the first 16 bytes of data are used.</param>
        /// <param name="ivLength">IV length</param>
        /// <returns>Decryption of data.</returns>
        public static byte[] AesDecrypt(ReadOnlyByteSpan data, byte[] key, byte[] iv = null, int ivLength = DefaultVectorLength)
        {
            if (iv == null)
            {
                iv = data[..ivLength];
                data = data[ivLength..];
            }

            using var aes = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, Mode = CipherMode.CBC, Key = key, IV = iv };
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write)) 
            {
                cs.Write(data);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Decrypted encrypted byte array.
        /// </summary>
        /// <param name="encrypted">encrypted byte array</param>
        /// <param name="password">password</param>
        /// <returns>decrypted message</returns>
        public static string AesDecrypt(byte[] encrypted, string password)
        {
            var (salt, key, iv, encrypt) = UnpackArrays(encrypted);

            ReadOnlyByteSpan keySpan = key;
            ReadOnlyByteSpan authKey = Encryption.KeyFromPassword(password, salt);

            if (authKey.Data.SequenceCompareTo(keySpan.Data) != 0)
            {
                throw new CryptographicException("Invalid signature");
            }

            var decrypt = Encryption.AesDecrypt(encrypt, key, iv);
            return Encoders.Utf8.Encode(decrypt);
        }

        #region Helpers

        private static IBufferedCipher CreateAes(byte[] key, byte[] iv)
        {
            var cipher = CipherUtilities.GetCipher(AesCryptoService);
            var keyParameters = CreateKeyParameters(key, iv);
            cipher.Init(true, keyParameters);
            return cipher;
        }

        private static ICipherParameters CreateKeyParameters(byte[] key, byte[] iv)
        {
            var keyParameter = new KeyParameter(key);
            return new ParametersWithIV(keyParameter, iv);
        }

        private static byte[] GenerateIV()
        {
            var random = new SecureRandom();
            return random.GenerateSeed(AesIvSize);
        }

        private static byte[] SaltBytes(int length = DefaultSaltLength)
        {
            var random = new SecureRandom(new DigestRandomGenerator(new Sha256Digest()));
            return random.GenerateSeed(length);
        }

        /// <summary>
        /// Pack encryption components. 
        /// </summary>
        /// <param name="salt">password salt</param>
        /// <param name="key">encryption key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="data">encrypted data</param>
        /// <returns>merged encrypted components</returns>
        private static byte[] PackArrays(byte[] salt, byte[] key, byte[] iv, byte[] data)
        {
            var arrays = new[] { salt, key, iv, data };
            var merge = new byte[arrays.Sum(a => a.Length) + arrays.Length * sizeof(int)];
            var index = 0;
            foreach (var a in arrays)
            {
                Array.Copy(BitConverter.GetBytes(a.Length), 0, merge, index, sizeof(int));
                index += sizeof(int);
                Array.Copy(a, 0, merge, index, a.Length);
                index += a.Length;
            }

            return merge;
        }

        /// <summary>
        /// Restore encryption components from merged encrypted data
        /// </summary>
        /// <param name="encryptedBytes">encrypted bytes</param>
        /// <returns>encrypted components</returns>
        private static (byte[] salt, byte[] key, byte[] iv, byte[] data) UnpackArrays(byte[] encryptedBytes)
        {
            var arrays = new byte[4][];

            var index = 0;
            for (var item = 0; item < arrays.Length; ++item)
            {
                var length = BitConverter.ToInt32(encryptedBytes, index);
                index += sizeof(int);
                arrays[item] = new byte[length];
                Array.Copy(encryptedBytes, index, arrays[item], 0, length);
                index += length;
            }

            return (arrays[0], arrays[1], arrays[2], arrays[3]);
        }

        #endregion
    }
}
