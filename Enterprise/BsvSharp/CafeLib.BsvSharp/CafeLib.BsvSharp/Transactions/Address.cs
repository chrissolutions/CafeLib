using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.BsvSharp.Network;

namespace CafeLib.BsvSharp.Transactions
{
    public class Address
    {
        private string _publicKeyHash;
        //private AddressType _addressType;
        private NetworkType _networkType;
        private int _version;

        /// Constructs a new Address object
        ///
        /// [address] is the base58encoded bitcoin address.
        ///
        public Address(string address)
        {
            //s_fromBase58(address);
        }


    }
}
