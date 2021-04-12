﻿using System;
using System.IO;
using System.Security.Cryptography;
using CafeLib.Bitcoin.Shared.Extensions;

namespace CafeLib.Bitcoin.Shared.Crypto
{
    public class Encrypt 
    {
        public static byte[] InitializationVector(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data, int length = 16)
            => Hashes.HmacSha256(key, data).ReadOnlySpan.Slice(0, length).ToArray();

        public static byte[] SaltBytes(int length = 8) 
        {
            var salt = new byte[length];
            using var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            return salt;
        }

        public static byte[] KeyFromPassword(NonNullable<string> password, byte[] salt = null, int iterations = 2048, int keyLength = 16)
            => KeyFromPassword(password.Value.Utf8ToBytes(), salt, iterations, keyLength);

        public static byte[] KeyFromPassword(NonNullable<byte[]> password, byte[] salt = null, int iterations = 2048, int keyLength = 16)
            => KeyFromPassword(password, HashAlgorithmName.SHA512, salt, iterations, keyLength);

        public static byte[] KeyFromPassword(NonNullable<byte[]> password, HashAlgorithmName algorithm, byte[] salt = null, int iterations = 2048, int keyLength = 16) 
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
        /// <param name="noIniVector">If true, the initialization vector used is not prepended to the encrypted result.</param>
        /// <returns>16 bytes of IV followed by encrypted data bytes.</returns>
        public static byte[] AesEncrypt(ReadOnlySpan<byte> data, byte[] key, byte[] iv = null, bool noIniVector = false)
        {
            using var aes = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, Mode = CipherMode.CBC, Key = key };
            if (iv == null) {
                aes.GenerateIV();
                iv = aes.IV;
            } else {
                aes.IV = iv;
            }

            byte[] r;

            using (var ms = new MemoryStream())
            {
                if (!noIniVector)
                    ms.Write(iv);
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write)) {
                    cs.Write(data);
                }
                r = ms.ToArray();
            }
            return r;
        }

        /// <summary>
        /// Applies AES decryption to data encrypted with AesEncrypt and the specified key.
        /// The actual iv used is obtained from the first 16 bytes of data if null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv">The IV to use. If null, the first 16 bytes of data are used.</param>
        /// <returns>Decryption of data.</returns>
        public static byte[] AesDecrypt(ReadOnlySpan<byte> data, byte[] key, byte[] iv = null)
        {
            if (iv == null) {
                iv = data.Slice(0, 16).ToArray();
                data = data[16..];
            }

            using var aes = new AesCryptoServiceProvider { Padding = PaddingMode.PKCS7, Mode = CipherMode.CBC, Key = key, IV = iv };

            byte[] r;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write)) {
                    cs.Write(data);
                }
                r = ms.ToArray();
            }
            return r;
        }
    }
}