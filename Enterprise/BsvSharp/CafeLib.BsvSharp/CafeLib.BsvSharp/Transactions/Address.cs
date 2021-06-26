using System;
using System.Linq;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Extensions;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Services;

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
        private static readonly HexEncoder Hex = Encoders.Hex;
        private static readonly Base58CheckEncoder Base58Check = Encoders.Base58Check;

        private byte[] _publicKeyAddress;

        /// <summary>
        /// Version property.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Address default constructor.
        /// </summary>
        public Address()
        {
        }

        /// Constructs a new Address object
        ///
        /// [address] is the base58encoded bitcoin address.
        ///
        public Address(string address)
        {
            FromBase58CheckInternal(address);
        }

        /// <summary>
        /// Constructs a new Address object from a base58-encoded string.
        ///
        /// Base58-encoded strings are the "standard" means of sharing bitcoin addresses amongst
        /// wallets. This is typically done either using the string of directly, or by using a
        /// QR-encoded form of this string.
        ///
        /// Typically, if someone is sharing their bitcoin address with you, this is the method
        /// you would use to instantiate an [Address] object for use with [Transaction] objects.
        /// </summary>
        /// <param name="base58Address"></param>
        /// <returns></returns>
        public static Address FromBase58(string base58Address) 
        {
            if (base58Address.Length == 25 || base58Address.Length == 34)
            {
                var address = new Address();
                address.FromBase58CheckInternal(base58Address);
                return address;
            }

            throw new FormatException("Address should be 25 bytes long. Only [${base58Address.length}] bytes long.");
        }

        /// Constructs a new Address object from a public key.
        ///
        /// [hexPubKey] is the hexadecimal encoding of a public key.
        ///
        /// [networkType] is used to distinguish between MAINNET and TESTNET.
        ///
        /// Also see [NetworkType]
        public static Address FromHex(string hexPubKey)
        {
            var address = new Address();
            address.FromHexInternal(hexPubKey);
            return address;
        }

        /// <summary>
        /// Constructs a new P2SH Address object from a script
        /// </summary>
        /// <param name="script"></param>
        /// <returns>address</returns>
        public static Address FromScript(Script script)
        {
            var address = new Address();
            address.FromScriptInternal(script);
            return address;
        }

        /// Serialize this address object to a base58-encoded string.
        /// This method is an alias for the [toBase58()] method
        public override string ToString() => Base58Check.Encode(_publicKeyAddress);

        /// Returns the public key hash `ripemd160(sha256(public_key))` encoded as a  hexadecimal string
        public string ToHex() => Hex.Encode(_publicKeyAddress);

        #region Helpers

        private void FromBase58CheckInternal(string source)
        {
            var versionAndDataBytes = Base58Check.Decode(source);
            Version = versionAndDataBytes[0];
            _publicKeyAddress = versionAndDataBytes[1..];
        }

        private void FromHexInternal(string hexPubKey)
        {
            Version = RootService.Network.PublicKeyAddress.First();
            _publicKeyAddress = Hex.Decode(hexPubKey).Hash160();
        }

        private void FromScriptInternal(Script script)
        {
            Version = RootService.Network.PublicKeyAddress.First();
            _publicKeyAddress = Hex.Decode(script.ToHexString()).Hash160();
        }

        #endregion
    }
}
