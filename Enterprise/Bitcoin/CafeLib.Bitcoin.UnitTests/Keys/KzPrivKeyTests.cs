#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Crypto;
using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Keys;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Keys
{
    public class PrivateKeyTests
    {
        [Fact]
        public void FromHexAndB58()
        {
            var hex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            var b58 = "L24Rq5hPWMexw5mQi7tchYw6mhtr5ApiHZMN8KJXCkskEv7bTV61";

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

        private const string StrSecret1 = "5HxWvvfubhXpYYpS3tJkw6fq9jE9j18THftkZjHHfmFiWtmAbrj";
        private const string StrSecret2 = "5KC4ejrDjv152FGwP386VD1i2NYc5KkfSMyv1nGy1VGDxGHqVY3";
        private const string StrSecret1C = "Kwr371tjA9u2rFSMZjTNun2PXXP3WPZu2afRHTcta6KxEUdm1vEw";
        private const string StrSecret2C = "L3Hq7a8FEQwJkW1M2GNKDW28546Vp5miewcCzSqUD9kCAXrJdS3g";
        private const string Addr1 = "1QFqqMUD55ZV3PJEJZtaKCsQmjLT6JkjvJ";
        private const string Addr2 = "1F5y5E5FMc5YzdJtB9hLaUe43GDxEKXENJ";
        private const string Addr1C = "1NoJrossxPBKfCHuJXT4HadJrXRE9Fxiqs";
        private const string Addr2C = "1CRj2HyM1CXWzHAXLQtiGLyggNT9WQqsDs";

        private const string StrAddressBad = "1HV9Lc3sNHZxwj4Zk6fB38tEmBryq2cBiF";

        [Fact]
        public void Base58PrivateKeyTests()
        {
            var bsecret1 = new Base58PrivateKey();
            var bsecret2 = new Base58PrivateKey();
            var bsecret1C = new Base58PrivateKey();
            var bsecret2C = new Base58PrivateKey();
            var baddress1 = new Base58PrivateKey();

            Assert.True(bsecret1.SetString(StrSecret1));
            Assert.True(bsecret2.SetString(StrSecret2));
            Assert.True(bsecret1C.SetString(StrSecret1C));
            Assert.True(bsecret2C.SetString(StrSecret2C));
            Assert.False(baddress1.SetString(StrAddressBad));

            var key1 = bsecret1.GetKey();
            Assert.False(key1.IsCompressed);
            var key2 = bsecret2.GetKey();
            Assert.False(key2.IsCompressed);
            var key1C = bsecret1C.GetKey();
            Assert.True(key1C.IsCompressed);
            var key2C = bsecret2C.GetKey();
            Assert.True(key2C.IsCompressed);

            var pubkey1 = key1.CreatePublicKey();
            var pubkey2 = key2.CreatePublicKey();
            var pubkey1C = key1C.CreatePublicKey();
            var pubkey2C = key2C.CreatePublicKey();

            var a = pubkey1.ToAddress();

            Assert.True(key1.VerifyPubKey(pubkey1));
            Assert.False(key1.VerifyPubKey(pubkey1C));
            Assert.False(key1.VerifyPubKey(pubkey2));
            Assert.False(key1.VerifyPubKey(pubkey2C));

            Assert.False(key1C.VerifyPubKey(pubkey1));
            Assert.True(key1C.VerifyPubKey(pubkey1C));
            Assert.False(key1C.VerifyPubKey(pubkey2));
            Assert.False(key1C.VerifyPubKey(pubkey2C));

            Assert.False(key2.VerifyPubKey(pubkey1));
            Assert.False(key2.VerifyPubKey(pubkey1C));
            Assert.True(key2.VerifyPubKey(pubkey2));
            Assert.False(key2.VerifyPubKey(pubkey2C));

            Assert.False(key2C.VerifyPubKey(pubkey1));
            Assert.False(key2C.VerifyPubKey(pubkey1C));
            Assert.False(key2C.VerifyPubKey(pubkey2));
            Assert.True(key2C.VerifyPubKey(pubkey2C));

            for (var n = 0; n < 16; n++) 
            {
                var strMsg = $"Very secret message {n}: 11";
                var hashMsg = Hashes.Hash256(strMsg.AsciiToBytes());

                // normal signatures

                var sign1 = key1.SignMessage(hashMsg);
                var sign1C = key1C.SignMessage(hashMsg);
                var sign2 = key2.SignMessage(hashMsg);
                var sign2C = key2C.SignMessage(hashMsg);
                Assert.NotNull(sign1);
                Assert.NotNull(sign1C);
                Assert.NotNull(sign2);
                Assert.NotNull(sign2C);

                Assert.True(pubkey1.Verify(hashMsg, sign1));
                Assert.True(pubkey1.Verify(hashMsg, sign1C));
                Assert.False(pubkey1.Verify(hashMsg, sign2));
                Assert.False(pubkey1.Verify(hashMsg, sign2C));

                Assert.True(pubkey1C.Verify(hashMsg, sign1));
                Assert.True(pubkey1C.Verify(hashMsg, sign1C));
                Assert.False(pubkey1C.Verify(hashMsg, sign2));
                Assert.False(pubkey1C.Verify(hashMsg, sign2C));

                Assert.False(pubkey2.Verify(hashMsg, sign1));
                Assert.False(pubkey2.Verify(hashMsg, sign1C));
                Assert.True(pubkey2.Verify(hashMsg, sign2));
                Assert.True(pubkey2.Verify(hashMsg, sign2C));

                Assert.False(pubkey2C.Verify(hashMsg, sign1));
                Assert.False(pubkey2C.Verify(hashMsg, sign1C));
                Assert.True(pubkey2C.Verify(hashMsg, sign2));
                Assert.True(pubkey2C.Verify(hashMsg, sign2C));

                // compact signatures (with key recovery)

                var compact1 = key1.CreateCompactSignature(hashMsg);
                var compact1C = key1C.CreateCompactSignature(hashMsg);
                var compact2 = key2.CreateCompactSignature(hashMsg);
                var compact2C = key2C.CreateCompactSignature(hashMsg);
                Assert.NotNull(compact1);
                Assert.NotNull(compact1C);
                Assert.NotNull(compact2);
                Assert.NotNull(compact2C);

            var rkey1 = PublicKey.FromRecoverCompact(hashMsg, compact1);
                var rkey2 = PublicKey.FromRecoverCompact(hashMsg, compact2);
                var rkey1C = PublicKey.FromRecoverCompact(hashMsg, compact1C);
                var rkey2C = PublicKey.FromRecoverCompact(hashMsg, compact2C);
                Assert.NotNull(rkey1);
                Assert.NotNull(rkey2);
                Assert.NotNull(rkey1C);
                Assert.NotNull(rkey2C);

                Assert.True(rkey1 == pubkey1);
                Assert.True(rkey2 == pubkey2);
                Assert.True(rkey1C == pubkey1C);
                Assert.True(rkey2C == pubkey2C);
            }
        }
    }
}
