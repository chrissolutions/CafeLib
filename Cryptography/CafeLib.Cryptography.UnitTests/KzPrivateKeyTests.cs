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

        [Fact]
        public void PrivateKey_From_Wif_Tests()
        {
            var hex = "96c132224121b509b7d0a16245e957d9192609c5637c6228311287b1be21627a";
            var wifLivenet = "L2Gkw3kKJ6N24QcDuH4XDqt9cTqsKTVNDGz1CRZhk9cq4auDUbJy";

            var privKey = new PrivateKey(hex);
            var privKey2 = PrivateKey.FromWif(wifLivenet);
            Assert.Equal(privKey, privKey2);
        }

        [Fact]
        public void PrivateKey_From_Wif_To_Hex_Test()
        {
            const string wifKeySource = "5HueCGU8rMjxEXxiPuD5BDku4MkFqeZyd4dZ1jvhTVqvbTLvyTJ";
            const string privateKeyHex = "0C28FCA386C7A227600B2FE50B7CAE11EC86D3BF1FBE471BE89827E19D72AA1D";

            var privKey = PrivateKey.FromWif(wifKeySource);
            var decodedPrivKey = privKey.ToHex().ToUpper();
            Assert.Equal(privateKeyHex, decodedPrivKey);
        }
    }
}