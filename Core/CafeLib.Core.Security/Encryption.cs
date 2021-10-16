using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using CafeLib.Core.Support;

namespace CafeLib.Core.Security
{
    public class Encryption
    {
        private const int DefaultVectorLength = 16;
        private const int DefaultKeyLength = 16;
        private const int DefaultSaltLength = 8;
        private const int DefaultIterations = 2048;

        public static byte[] InitializationVector(byte[] key, byte[] data, int length = DefaultVectorLength)
        {
            return new HMACSHA256(key).TransformFinalBlock(data, 0,length);
        }

        public static byte[] SaltBytes(int length = DefaultSaltLength)
        {
            var salt = new byte[length];
            using var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            return salt;
        }

        public static byte[] KeyFromPassword(NonNullable<string> password, byte[] salt = null, int iterations = DefaultIterations, int keyLength = DefaultKeyLength)
            => KeyFromPassword(Encoding.UTF8.GetBytes(password.Value), salt, iterations, keyLength);

        public static byte[] KeyFromPassword(NonNullable<byte[]> password, byte[] salt = null, int iterations = DefaultIterations, int keyLength = DefaultKeyLength)
            => KeyFromPassword(password, HashAlgorithmName.SHA512, salt, iterations, keyLength);

        public static byte[] KeyFromPassword(NonNullable<byte[]> password, HashAlgorithmName algorithm, byte[] salt = null, int iterations = DefaultIterations, int keyLength = DefaultKeyLength)
        {
            salt ??= SaltBytes();
            using var keyGen = new Rfc2898DeriveBytes(password, salt, iterations, algorithm);
            return keyGen.GetBytes(keyLength);
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
        public static byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv = null, bool noInitVector = false)
        {
            using var aes = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, Mode = CipherMode.CBC, Key = key };
            using var ms = new MemoryStream();
            if (iv == null)
            {
                aes.GenerateIV();
                iv = aes.IV;
            }
            else
            {
                aes.IV = iv;
            }

            if (!noInitVector)
            {
                ms.Write(iv);
            }

            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                cs.Write(data);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Encrypted message
        /// </summary>
        /// <param name="message">message to be encrypted</param>
        /// <param name="secret">secret</param>
        /// <returns></returns>
        public static byte[] AesEncrypt(NonNullable<string> message, NonNullable<string> secret)
        {
            var bytes = Encoding.UTF8.GetBytes(message.Value);
            var keySalt = SaltBytes();
            var key = KeyFromPassword(secret, keySalt);
            var iv = InitializationVector(key, bytes);
            var data = AesEncrypt(bytes, key, iv, true);

            return MergeArrays(keySalt, key, iv, data);
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
        public static byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv = null, int ivLength = DefaultVectorLength)
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
        /// <param name="secret">secret</param>
        /// <returns>decrypted message</returns>
        public static string AesDecrypt(byte[] encrypted, string secret)
        {
            var (salt, key, iv, encrypt) = RestoreArrays(encrypted);

            var authKey = KeyFromPassword(secret, salt);
            if (authKey.AsSpan().SequenceCompareTo(key) != 0)
            {
                throw new SecurityException("Secret not authenticated");
            }

            var decrypt = AesDecrypt(encrypt, key, iv);
            return Encoding.UTF8.GetString(decrypt);
        }

        #region Helpers

        /// <summary>
        /// Merge encryption components. 
        /// </summary>
        /// <param name="salt">password salt</param>
        /// <param name="key">encryption key</param>
        /// <param name="iv">initialization vector</param>
        /// <param name="data">encrypted data</param>
        /// <returns>merged encrypted components</returns>
        private static byte[] MergeArrays(byte[] salt, byte[] key, byte[] iv, byte[] data)
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
        private static (byte[] salt, byte[] key, byte[] iv, byte[] data) RestoreArrays(byte[] encryptedBytes)
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
