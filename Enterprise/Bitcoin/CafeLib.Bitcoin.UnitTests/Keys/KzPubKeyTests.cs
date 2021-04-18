#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Keys;
using Xunit;

namespace CafeLib.Bitcoin.UnitTests.Keys
{
    public class KzPubKeyTests
    {
        [Fact]
        public void FromPrivateKey()
        {
            var privhex = "906977a061af29276e40bf377042ffbde414e496ae2260bbf1fa9d085637bfff";
            var pubhex = "02a1633cafcc01ebfb6d78e39f687a1f0995c62fc95f51ead10a02ee0be551b5dc";

            var privkey = PrivateKey.FromHex(privhex);
            var pubkey = privkey.CreatePublicKey();
            Assert.Equal(privhex, privkey.ToHex());
            Assert.Equal(pubhex, pubkey.ToHex());
            Assert.True(privkey.VerifyPubKey(pubkey));
        }
    }
}
