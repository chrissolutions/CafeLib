using System.Security;
using System.Text;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class EncryptionTests
    {
        [Fact]
        public void AesEncryptDecryptTests()
        {
            var msg = "all good men must act";
            var data1 = Encoding.UTF8.GetBytes(msg);
            var password = "really strong password...;-)";

            var key = AesEncryption.KeyFromPassword(password);

            var edata1 = AesEncryption.Encrypt(data1, key);
            var ddata1 = AesEncryption.Decrypt(edata1, key);
            Assert.Equal(data1, ddata1);
            Assert.Equal(msg, Encoding.UTF8.GetString(ddata1));
        }

        [Fact]
        public void AesEncryptDecryptTest_With_InitializationVector()
        {
            var msg = "all good men must act";
            var data1 = Encoding.UTF8.GetBytes(msg);
            var password = "really strong password...;-)";

            var key = AesEncryption.KeyFromPassword(password);

            var iv = AesEncryption.InitializationVector(key, data1);
            var edata1 = AesEncryption.Encrypt(data1, key, iv, true);
            var ddata1 = AesEncryption.Decrypt(edata1, key, iv);
            Assert.Equal(data1, ddata1);
            Assert.Equal(msg, Encoding.UTF8.GetString(ddata1));
        }

        [Fact]
        public void AesEncryptStringTests()
        {
            const string msg = "all good men must act";
            const string password = "really strong password...;-)";

            var encrypt = AesEncryption.Encrypt(msg, password);
            var decrypt = AesEncryption.Decrypt(encrypt, password);
            Assert.Equal(msg, decrypt);
        }

        [Fact]
        public void AesEncryptStringTests_BadPassword()
        {
            const string msg = "all good men must act";
            const string password = "really strong password...;-)";

            var encrypt = AesEncryption.Encrypt(msg, password);
            Assert.Throws<SecurityException>(() => AesEncryption.Decrypt(encrypt, "Bad password"));
            var decrypt = AesEncryption.Decrypt(encrypt, password);
            Assert.Equal(msg, decrypt);
        }
    }
}