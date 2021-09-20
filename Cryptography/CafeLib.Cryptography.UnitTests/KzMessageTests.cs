using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Cryptography.UnitTests.BsvSharp.Encoding;
using CafeLib.Cryptography.UnitTests.BsvSharp.Keys;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class KzMessageTests
    {
        [Theory]
        [InlineData(
            "15jZVzLc9cXz5PUFFda5A4Z7kZDYPg2NnL",
            "L3TiCqmvPkXJpzCCZJuhy6wQtJZWDkR1AuqFY4Utib5J5XLuvLdZ",
            "This is an example of a signed message.",
            "H6sliOnVrD9r+J8boZAKHZwBIW2zLiD72IfTIF94bfZhBI0JdMu9AM9rrF7P6eH+866YvM4H9xWGVN4jMJZycFU="
        )]
        public void VerifyMessage_Test(string address, string privateKey, string message, string signature)
        {
            var addr = new Address(address);

            var base58 = Encoders.Base58.Decode(privateKey);
            var privKey = new PrivateKey(base58);
            //var privKey.SetData()

            //    var signature = secret.PrivateKey.SignMessage(test.Message);
            //    Assert.True(pkh.VerifyMessage(test.Message, signature));
            //    Assert.True(secret.PubKey.VerifyMessage(test.Message, signature));
            //}
            //Assert.True(pkh.VerifyMessage(test.Message, test.Signature));
            //Assert.True(!pkh.VerifyMessage("bad message", test.Signature));
        }
    }
}
