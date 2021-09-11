#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Crypto;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Encrypt
{
    public class KzEncryptTests
    {
        [Fact]
        public void Aes_Encrypt_Decrypt()
        {
            var msg = "all good men must act";
            var password = "really strong password...;-)";

            var key = AesEncryption.KeyFromPassword(password);

            var encrypted = AesEncryption.Encrypt(msg, key);
            var decrypted = AesEncryption.Decrypt(encrypted, key);
            Assert.Equal(msg, decrypted);
        }

        [Fact]
        public void Aes_Encrypt_Decrypt_With_IV()
        {
            var msg = "all good men must act";
            var data1 = msg.Utf8ToBytes();
            var password = "really strong password...;-)";

            var key = Encryption2.KeyFromPassword(password);

            var iv = Encryption2.InitializationVector(key, data1);
            var edata1 = Encryption2.AesEncrypt(data1, key, iv, true);
            var ddata1 = Encryption2.AesDecrypt(edata1, key, iv);
            Assert.Equal(data1, ddata1);
            Assert.Equal(msg, Encoders.Utf8.Encode(ddata1));
        }


        //[Fact]
        //public void AesEncryptStringTests()
        //{
        //    const string msg = "all good men must act";
        //    const string password = "really strong password...;-)";

        //    var encrypt = Encryption.AesEncrypt(msg, password);
        //    var decrypt = Encryption.AesDecrypt(encrypt, password);
        //    Assert.Equal(msg, decrypt);
        //}

        //[Fact]
        //public void AesEncryptStringTests_BadPassword()
        //{
        //    const string msg = "all good men must act";
        //    const string password = "really strong password...;-)";

        //    var encrypt = Encryption.AesEncrypt(msg, password);
        //    Assert.Throws<CryptographicException>(() => Encryption.AesDecrypt(encrypt, "Bad password"));
        //    var decrypt = Encryption.AesDecrypt(encrypt, password);
        //    Assert.Equal(msg, decrypt);
        //}
    }
}