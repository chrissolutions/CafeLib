using System;
using CafeLib.BsvSharp.BouncyCastle.Crypto;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Digests;
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
    public static class AesEncryption
    {
        private const string Algorithm = "AES";

        private const int DefaultVectorLength = 16;
        private const int DefaultKeyLength = 16;
        private const int DefaultSaltLength = 8;
        private const int DefaultIterations = 2048;

        private const byte AesIvSize = 16;

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

        private static readonly string AesCryptoService = $"{Algorithm}/{AesTypes.CipherMode.CBC}/{AesTypes.Padding.PKCS7}";

        public static string Encrypt(string plainText, byte[] key)
        {
            var iv = GenerateIV();
            var plainTextData = plainText.Utf8ToBytes();
            var data = Encrypt(plainTextData, key, iv);
            return Convert.ToBase64String(data);
        }

        public static byte[] Encrypt(ReadOnlyByteSpan data, byte[] key, byte[] iv = null)
        {
            iv ??= GenerateIV();
            var keyParameters = CreateKeyParameters(key, iv);
            var cipher = CipherUtilities.GetCipher(AesCryptoService);
            cipher.Init(true, keyParameters);
            var cipherData = cipher.DoFinal(data);
            return PackCipherData(cipherData, iv);
        }

        public static string Decrypt(string cipherText, byte[] key)
        {
            var cipherData = Convert.FromBase64String(cipherText);
            var (encryptedBytes, iv) = UnpackCipherData(cipherData);
            var decryptedData = Decrypt(encryptedBytes, key, iv);
            return Encoders.Utf8.Encode(decryptedData);
        }

        public static byte[] Decrypt(ReadOnlyByteSpan data, byte[] key, byte[] iv = null, int ivLength = DefaultVectorLength)
        {
            if (iv == null)
            {
                iv = data[..ivLength];
                data = data[ivLength..];
            }

            var keyParameters = CreateKeyParameters(key, iv);
            var cipher = CipherUtilities.GetCipher(AesCryptoService);
            cipher.Init(false, keyParameters);
            var decryptedData = cipher.DoFinal(data);
            return decryptedData;
        }

        #region Helpers

        private static byte[] SaltBytes(int length = DefaultSaltLength)
        {
            var random = new SecureRandom(new DigestRandomGenerator(new Sha256Digest()));
            return random.GenerateSeed(length);
        }

        private static ICipherParameters CreateKeyParameters(byte[] key, byte[] iv)
        {
            var keyParameter = new KeyParameter(key);
            return new ParametersWithIV(keyParameter, iv);
        }

        private static byte[] PackCipherData(byte[] encryptedBytes, byte[] iv)
        {
            var dataSize = encryptedBytes.Length + iv.Length + 1;
            var index = 0;
            var data = new byte[dataSize];
            data[index] = AesIvSize;
            index += 1;

            Array.Copy(iv, 0, data, index, iv.Length);
            index += iv.Length;
            Array.Copy(encryptedBytes, 0, data, index, encryptedBytes.Length);

            return data;
        }

        private static (byte[], byte[]) UnpackCipherData(byte[] cipherData)
        {
            var index = 0;
            var ivSize = cipherData[index];
            index += 1;

            var iv = new byte[ivSize];
            Array.Copy(cipherData, index, iv, 0, ivSize);
            index += ivSize;

            var encryptedBytes = new byte[cipherData.Length - index];
            Array.Copy(cipherData, index, encryptedBytes, 0, encryptedBytes.Length);
            return (encryptedBytes, iv);
        }

        private static (byte[], byte[]) UnpackCipherData(string cipherText)
        {
            var index = 0;
            var cipherData = Convert.FromBase64String(cipherText);
            var ivSize = cipherData[index];
            index += 1;

            var iv = new byte[ivSize];
            Array.Copy(cipherData, index, iv, 0, ivSize);
            index += ivSize;

            var encryptedBytes = new byte[cipherData.Length - index];
            Array.Copy(cipherData, index, encryptedBytes, 0, encryptedBytes.Length);
            return (encryptedBytes, iv);
        }

        private static byte[] GenerateIV()
        {
            var random = new SecureRandom();
            return random.GenerateSeed(AesIvSize);
        }

        #endregion
    }
}
