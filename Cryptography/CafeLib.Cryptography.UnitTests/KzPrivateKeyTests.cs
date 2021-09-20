using CafeLib.Cryptography.UnitTests.BsvSharp.Extensions;
using CafeLib.Cryptography.UnitTests.BsvSharp.Keys;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class KzPrivateKeyTests
    {
        [Fact]
        public void FromHexAndB58()
        {
            const string hex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            const string b58 = "L24Rq5hPWMexw5mQi7tchYw6mhtr5ApiHZMN8KJXCkskEv7bTV61";

            var key1 = new PrivateKey(hex);
            var key2 = PrivateKey.FromBase58(b58);
            Assert.Equal(key1, key2);
            Assert.Equal(hex, key1.ToHex());
            Assert.Equal(b58, key1.ToBase58().ToString());
            Assert.Equal(b58, key1.ToString());
            Assert.Equal(hex, key2.ToHex());
            Assert.Equal(b58, key2.ToBase58().ToString());
            Assert.Equal(b58, key2.ToString());
        }

        [Fact]
        public void PublicKey_From_PrivateKey_Test()
        {
            const string hex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            const string b58 = "L24Rq5hPWMexw5mQi7tchYw6mhtr5ApiHZMN8KJXCkskEv7bTV61";
            const string publicKey = "17JarKo61PkpuZG3GyofzGmFSCskGRBUT3";
            const string pubHex = "02a1633cafcc01ebfb6d78e39f687a1f0995c62fc95f51ead10a02ee0be551b5dc";

            var key1 = new PrivateKey(hex);
            var key2 = PrivateKey.FromBase58(b58);

            var pubKey1 = key1.CreatePublicKey();
            var pubKey2 = key2.CreatePublicKey();

            Assert.Equal(pubHex, pubKey1.ToHex());
            Assert.Equal(pubHex, pubKey2.ToHex());

            Assert.Equal(pubKey1, pubKey2);
            Assert.Equal(publicKey, pubKey1.ToString());
            Assert.Equal(publicKey, pubKey2.ToString());
        }
    }
}