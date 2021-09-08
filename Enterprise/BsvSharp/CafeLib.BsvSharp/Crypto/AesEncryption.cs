using System;
using CafeLib.BsvSharp.BouncyCastle.Crypto;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Parameters;
using CafeLib.BsvSharp.BouncyCastle.Security;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;

namespace CafeLib.BsvSharp.Crypto
{
    public static class AesEncryption
    {
        private const string Algorithm = "AES";

        private const byte AesIvSize = 16;

        private static readonly string AesCryptoService = $"{Algorithm}/{AesTypes.CipherMode.CBC}/{AesTypes.Padding.PKCS7}";

        public static string Encrypt(string plainText, string key) => Encrypt(plainText, key.Utf8ToBytes());

        public static string Encrypt(string plainText, byte[] key)
        {
            var random = new SecureRandom();
            var iv = random.GenerateSeed(AesIvSize);
            var keyParameters = CreateKeyParameters(key, iv);
            var cipher = CipherUtilities.GetCipher(AesCryptoService);
            cipher.Init(true, keyParameters);

            var plainTextData = plainText.Utf8ToBytes();
            var cipherText = cipher.DoFinal(plainTextData);

            return PackCipherData(cipherText, iv);
        }

        public static string Decrypt(string cipherText, string key) => Decrypt(cipherText, key.Utf8ToBytes());

        public static string Decrypt(string cipherText, byte[] key)
        {
            var (encryptedBytes, iv)  = UnpackCipherData(cipherText);
            var keyParameters = CreateKeyParameters(key, iv);
            var cipher = CipherUtilities.GetCipher(AesCryptoService);
            cipher.Init(false, keyParameters);

            var decryptedData = cipher.DoFinal(encryptedBytes);
            return Encoders.Utf8.Encode(decryptedData);
        }

        #region Helpers

        private static ICipherParameters CreateKeyParameters(byte[] key, byte[] iv)
        {
            var keyParameter = new KeyParameter(key);
            return new ParametersWithIV(keyParameter, iv);
        }

        private static string PackCipherData(byte[] encryptedBytes, byte[] iv)
        {
            var dataSize = encryptedBytes.Length + iv.Length + 1;
            var index = 0;
            var data = new byte[dataSize];
            data[index] = AesIvSize;
            index += 1;

            Array.Copy(iv, 0, data, index, iv.Length);
            index += iv.Length;
            Array.Copy(encryptedBytes, 0, data, index, encryptedBytes.Length);

            return Convert.ToBase64String(data);
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

        #endregion
    }
}
