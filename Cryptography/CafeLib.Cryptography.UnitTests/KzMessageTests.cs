﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CafeLib.Cryptography.UnitTests.BsvSharp.Encoding;
using CafeLib.Cryptography.UnitTests.BsvSharp.Extensions;
using CafeLib.Cryptography.UnitTests.BsvSharp.Keys;
using CafeLib.Cryptography.UnitTests.BsvSharp.Signatures;
using Xunit;

namespace CafeLib.Cryptography.UnitTests
{
    public class KzMessageTests
    {
        [Fact]
        public void SignMessageTest()
        {
            const string message = "147@moneybutton.com02019-06-07T20:55:57.562ZPayment with Money Button";
            const string signature = "HxjyaWDKtUXXN78HOpVwK9xTuIjtP2AZeOTKrbo/PnBJMa4qVhDiyhzulBL89zJnp0sxqq4hpt6mUmGrd/Q/R2U=";

            var privateKey = PrivateKey.FromWif("L3nrwRssVKMkScjejmmu6kmq4hSuUApJnFdW1hGvBP69jnQuKYCh");
            var sig = privateKey.SignMessageToBase64(message);
            Assert.Equal(signature, sig);

            var ok = privateKey.CreatePublicKey().VerifyMessage(message, Signature.FromBase64(sig));
            Assert.True(ok);
        }

        [Fact]
        public void SignMessage_With_PublicKey_Test()
        {
            const string pubKeyHex = "02e36811b6a8db1593aa5cf97f91dd2211af1c38b9890567e58367945137dca8ef";

            const string message = "147@moneybutton.com02019-06-07T20:55:57.562ZPayment with Money Button";
            const string signature = "HxjyaWDKtUXXN78HOpVwK9xTuIjtP2AZeOTKrbo/PnBJMa4qVhDiyhzulBL89zJnp0sxqq4hpt6mUmGrd/Q/R2U=";
            const string sign = "H4Q8tvj632hXiirmiiDJkuUN9Z20zDu3KaFuwY8cInZiLhgVJKJdKrZx1RZN06E/AARnFX7Fn618OUBQigCis4M=";

            var privateKey = PrivateKey.FromWif("L3nrwRssVKMkScjejmmu6kmq4hSuUApJnFdW1hGvBP69jnQuKYCh");
            var sig = privateKey.SignMessageToBase64(message);
            Assert.Equal(signature, sig);

            var publicKey = privateKey.CreatePublicKey();
            var ok = publicKey.VerifyMessage(message, Signature.FromBase64(sig));
            Assert.True(ok);

            var pubKey = PublicKey.FromHex(pubKeyHex);
            ok = pubKey.VerifyMessage(message, sign);
        }


        [Fact]
        public void VerifyMessageSignatureTest()
        {
            const string message = "This is an example of a signed message.";
            const string signature = "H6sliOnVrD9r+J8boZAKHZwBIW2zLiD72IfTIF94bfZhBI0JdMu9AM9rrF7P6eH+866YvM4H9xWGVN4jMJZycFU=";

            var publicKey = PublicKey.FromMessage(message, signature);
            var ok = publicKey.VerifyMessage(message, signature);
            Assert.True(ok);
        }

        [Theory]
        [InlineData(
            "15jZVzLc9cXz5PUFFda5A4Z7kZDYPg2NnL",
            "L3TiCqmvPkXJpzCCZJuhy6wQtJZWDkR1AuqFY4Utib5J5XLuvLdZ",
            "This is an example of a signed message.",
            "H6sliOnVrD9r+J8boZAKHZwBIW2zLiD72IfTIF94bfZhBI0JdMu9AM9rrF7P6eH+866YvM4H9xWGVN4jMJZycFU="
        )]
        [InlineData(
            "1QFqqMUD55ZV3PJEJZtaKCsQmjLT6JkjvJ",
            "5HxWvvfubhXpYYpS3tJkw6fq9jE9j18THftkZjHHfmFiWtmAbrj",
            "hello world",
            "G+dnSEywl3v1ijlWXvpY6zpu+AKNNXJcVmrdE35m0mMlzwFzXDiNg+uZrG9k8mpQL6sjHKrlBoDNSA+yaPW7PEA="
        )]
        [InlineData(
            "1GvPJp7H8UYsYDvE4GFoV4f2gSCNZzGF48",
            "5JEeah4w29axvf5Yg9v9PKv86zcCN9qVbizJDMHmiSUxBqDFoUT",
            "This is an example of a signed message2",
            "G8YNwlo+I36Ct+hZKGSBFl3q8Kbx1pxPpwQmwdsG85io76+DUOHXqh/DfBq+Cn2R3C3dI//g3koSjxy7yNxJ9m8="
        )]
        [InlineData(
            "1GvPJp7H8UYsYDvE4GFoV4f2gSCNZzGF48",
            "5JEeah4w29axvf5Yg9v9PKv86zcCN9qVbizJDMHmiSUxBqDFoUT",
            "this is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long messagethis is a very long message",
            "HFKBHewleUsotk6fWG0OvWS/E2pP4o5hixdD6ui60in/x4376FBI4DvtJYrljXLNJTG1pBOZG+qRT/7S9WiIBfQ="
        )]
        //[InlineData(
        //    "1Q1wVsNNiUo68caU7BfyFFQ8fVBqxC2DSc",
        //    null,
        //    "Localbitcoins.com will change the world",
        //    "IJ/17TjGGUqmEppAliYBUesKHoHzfY4gR4DW0Yg7QzrHUB5FwX1uTJ/H21CF8ncY8HHNB5/lh8kPAOeD5QxV8Xc="
        //)]
        public void VerifyMessage_Test(string address, string privateKey, string message, string signature)
        {
            var addr = new Address(address);
            var privKey = PrivateKey.FromBase58(privateKey);
            var sign = new Signature(Encoders.Base64.Decode(signature));

            var pubKey = privKey.CreatePublicKey();
            Assert.Equal(addr.PubKeyHash, pubKey.GetId());
            Assert.True( pubKey.VerifyMessage(message, sign));
        }
    }
}
