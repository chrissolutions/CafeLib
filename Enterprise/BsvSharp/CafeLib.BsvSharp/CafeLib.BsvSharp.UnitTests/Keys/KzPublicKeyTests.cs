#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Keys;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Keys
{
    public class KzPublicKeyTests
    {
        [Fact]
        public void FromPrivateKey()
        {
            const string privateHex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            const string publicHex = "02a1633cafcc01ebfb6d78e39f687a1f0995c62fc95f51ead10a02ee0be551b5dc";

            var privkey = PrivateKey.FromHex(privateHex);
            var pubkey = privkey.CreatePublicKey();
            Assert.Equal(privateHex, privkey.ToHex());
            Assert.Equal(publicHex, pubkey.ToHex());
            Assert.True(privkey.VerifyPubKey(pubkey));
        }

        [Fact]
        public void TestPublicKeyData()
        {
            const string publicHex = "02a1633cafcc01ebfb6d78e39f687a1f0995c62fc95f51ead10a02ee0be551b5dc";
            var publicKey = new PublicKey(publicHex);
            var bytes = publicKey.ToArray();
            Assert.Equal(0, publicKey.Data.SequenceCompareTo(bytes));
        }

        [Fact]
        public void TestPublicKeyAddress()
        {
            const string privateHex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            const string publicHex = "02a1633cafcc01ebfb6d78e39f687a1f0995c62fc95f51ead10a02ee0be551b5dc";
            const string publicKey = "17JarKo61PkpuZG3GyofzGmFSCskGRBUT3";

            var privkey = PrivateKey.FromHex(privateHex);
            var pubkey = privkey.CreatePublicKey();
            var address = pubkey.ToAddress();

            Assert.Equal(privateHex, privkey.ToHex());
            Assert.Equal(publicHex, pubkey.ToHex());
            Assert.True(privkey.VerifyPubKey(pubkey));
            Assert.Equal(publicKey, address.ToString());
        }
    }
}