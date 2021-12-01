using System.Security;
using System.Text;
using CafeLib.Core.Security;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class EncryptionTests
    {
        [Fact]
        public void AesEncryptDecryptTests()
        {
            var msg = "all good men must act";
            var data1 = Encoding.UTF8.GetBytes(msg);
            var password = "really strong password...;-)";

            var key = Encryption.KeyFromPassword(password);

            var edata1 = Encryption.AesEncrypt(data1, key);
            var ddata1 = Encryption.AesDecrypt(edata1, key);
            Assert.Equal(data1, ddata1);
            Assert.Equal(msg, Encoding.UTF8.GetString(ddata1));
        }

        [Fact]
        public void AesEncryptDecryptTest_With_InitializationVector()
        {
            var msg = "all good men must act";
            var data1 = Encoding.UTF8.GetBytes(msg);
            var password = "really strong password...;-)";

            var key = Encryption.KeyFromPassword(password);

            var iv = Encryption.InitializationVector(key, data1);
            var edata1 = Encryption.AesEncrypt(data1, key, iv, true);
            var ddata1 = Encryption.AesDecrypt(edata1, key, iv);
            Assert.Equal(data1, ddata1);
            Assert.Equal(msg, Encoding.UTF8.GetString(ddata1));
        }

        [Fact]
        public void AesEncryptStringTests()
        {
            const string msg = "all good men must act";
            const string password = "really strong password...;-)";

            var encrypt = Encryption.AesEncrypt(msg, password);
            var decrypt = Encryption.AesDecrypt(encrypt, password);
            Assert.Equal(msg, decrypt);
        }

        [Fact]
        public void AesEncryptStringTests_BadPassword()
        {
            const string msg = "all good men must act";
            const string password = "really strong password...;-)";

            var encrypt = Encryption.AesEncrypt(msg, password);
            Assert.Throws<SecurityException>(() => Encryption.AesDecrypt(encrypt, "Bad password"));
            var decrypt = Encryption.AesDecrypt(encrypt, password);
            Assert.Equal(msg, decrypt);
        }
    }
}