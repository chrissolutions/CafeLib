#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Network;
using CafeLib.BsvSharp.Transactions;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Chain
{
    public class KzAddressTests
    {
        [Theory]
        [InlineData("13k3vneZ3yvZnc9dNWYH2RJRFsagTfAERv", AddressType.PubkeyHash, NetworkType.Main)]
        [InlineData("15vkcKf7gB23wLAnZLmbVuMiiVDc1Nm4a2", AddressType.PubkeyHash, NetworkType.Main)]
        [InlineData("1A6ut1tWnUq1SEQLMr4ttDh24wcbJ5o9TT", AddressType.PubkeyHash, NetworkType.Main)]
        [InlineData("1BpbpfLdY7oBS9gK7aDXgvMgr1DPvNhEB2", AddressType.PubkeyHash, NetworkType.Main)]
        [InlineData("1Jz2yCRd5ST1p2gUqFB5wsSQfdm3jaFfg7", AddressType.PubkeyHash, NetworkType.Main)]
        [InlineData("n28S35tqEMbt6vNad7A5K3mZ7vdn8dZ86X", AddressType.PubkeyHash, NetworkType.Test)]
        public void AddressToStringTest(string base58, AddressType addressType, NetworkType networkType)
        {
            var address = new Address(base58);
            Assert.Equal(base58, address.ToString());
            Assert.Equal(addressType, address.AddressType);
            Assert.Equal(networkType, address.NetworkType);
        }
    }
}
