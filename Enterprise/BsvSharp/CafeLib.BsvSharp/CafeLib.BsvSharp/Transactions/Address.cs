using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Network;

namespace CafeLib.BsvSharp.Transactions
{
    /// <summary>
    /// This class abstracts away the internals of address encoding and provides
    /// a convenient means to both encode and decode information from a bitcoin address.
    ///
    /// Bitcoin addresses are a construct which facilitates interoperability
    /// between different wallets. I.e. an agreement amongst wallet providers to have a
    /// common means of sharing the hashed public key value needed to send someone bitcoin using one
    /// of the standard public-key-based transaction types.
    ///
    /// The Address does not contain a public key, only a hashed value of a public key
    /// which is derived as explained below.
    ///
    /// Bitcoin addresses are not part of the consensus rules of bitcoin.
    ///
    /// Bitcoin addresses are encoded as follows
    /// * 1st byte - indicates the network type which is either MAINNET or TESTNET
    /// * next 20 bytes - the hash value computed by taking the `ripemd160(sha256(PUBLIC_KEY))`
    /// * last 4 bytes  - a checksum value taken from the first four bytes of sha256(sha256(previous_21_bytes))
    /// </summary>
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

        private void FromBase58(string address)
        {
            address = address.Trim();
            var versionAndDataBytes = Encoders.Base58Check.Decode(address);
            var versionByte = versionAndDataBytes[0];

            _version = versionByte & 0xFF;
            //_networkTypes = Networks.getNetworkTypes(_version);
            //_addressType = Networks.getAddressType(_version);
            //_networkType = Networks.getNetworkTypes(_version)[0];
            //var stripVersion = versionAndDataBytes.sublist(1, versionAndDataBytes.length);
            //_publicKeyHash = HEX.encode(stripVersion.map((elem) => elem.toUnsigned(8)).toList());
        }
    }
}
