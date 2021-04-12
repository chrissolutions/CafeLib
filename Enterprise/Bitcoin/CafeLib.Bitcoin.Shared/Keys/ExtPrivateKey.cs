#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CafeLib.Bitcoin.Shared.Buffers;
using CafeLib.Bitcoin.Shared.Crypto;
using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Services;

namespace CafeLib.Bitcoin.Shared.Keys
{
    public class ExtPrivateKey : ExtKey
    {
        public PrivateKey PrivateKey { get; private set; } = new PrivateKey();

        /// <summary>
        /// Sets this extended private key to be a master (depth 0) with the given private key and chaincode and verifies required key paths.
        /// </summary>
        /// <param name="privkey">Master private key.</param>
        /// <param name="chaincode">Master chaincode.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <returns>Returns this key unless required key paths aren't valid for specified key.</returns>
        public ExtPrivateKey SetMaster(UInt256 privkey, UInt256 chaincode, IEnumerable<KeyPath> required = null)
        {
            PrivateKey = new PrivateKey(privkey);
            ChainCode = chaincode;
            Depth = 0;
            Child = 0;
            Fingerprint = 0;

            if (PrivateKey == null || !PrivateKey.IsValid) goto fail;

            // Verify that all the required derivation paths yield valid keys.
            if (required == null) return this;
            foreach (var r in required) 
                if (Derive(r) == null) 
                    goto fail;

            return this;

        fail:
            return null;
        }

        /// <summary>
        /// Sets this extended private key to be a master (depth 0) with the private key and chaincode set from the single 512 bit vout parameter.
        /// Master private key will be set to the first 256 bits.
        /// Chaincode will be set from the last 256 bits.
        /// </summary>
        /// <param name="vout">Master private key will be set to the first 256 bits. Chaincode will be set from the last 256 bits.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the specified key or returns null.</param>
        /// <returns>Returns this key unless required key paths aren't valid for specified key.</returns>
        public ExtPrivateKey SetMaster(UInt512 vout, IEnumerable<KeyPath> required = null)
        {
            return SetMaster(vout.ReadOnlySpan.Slice(0, 32), vout.ReadOnlySpan.Slice(32, 32), required);
        }

        /// <summary>
        /// Sets Bip32 private key.
        /// Uses a single invocation of HMACSHA512 to generate 512 bits of entropy with which to set master private key and chaincode.
        /// </summary>
        /// <param name="hmacData">Sequence of bytes passed as hmacData to HMACSHA512 along with byte encoding of hmacKey.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="hmacKey">Default is current global Kz.MasterBip32Key which may default to "Bitcoin seed".</param>
        /// <returns>Returns this key unless required key paths aren't valid for generated key.</returns>
        public ExtPrivateKey SetMasterBip32(ReadOnlySpan<byte> hmacData, IEnumerable<KeyPath> required = null, string hmacKey = null)
        {
            hmacKey ??= RootService.MasterBip32Key;
            var vout = Hashes.HmacSha512(hmacKey.Utf8NormalizedToBytes(), hmacData);
            return SetMaster(vout, required);
        }

        /// <summary>
        /// Sets hybrid Bip32 / Bip39 private key.
        /// Uses only a single Bip32 style use of HMACSHA512 starting with at least 32 bytes of Bip39 entropy from mnemonicWords.
        /// </summary>
        /// <param name="mnemonicWords">Must be at least 32 bytes of Bip39 mnemonic word entropy with valid checksum.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="hmacKey">Default is current global Kz.MasterBip32Key which may default to "Bitcoin seed".</param>
        /// <returns>Returns this key unless required key paths aren't valid for generated key.</returns>
        public ExtPrivateKey SetMasterBip32(string mnemonicWords, IEnumerable<KeyPath> required = null, string hmacKey = null)
        {
            //var e = KzMnemonic.FromWords(mnemonicWords).Entropy;
            //if (e == null || e.Length < 32)
            //    throw new ArgumentException($"{nameof(mnemonicWords)} must provide at least 32 bytes of BIP39 mnemonic entropy.");
            //return SetMasterBip32(e, required, hmacKey);
            return null;
        }

        /// <summary>
        /// Computes 512 bit Bip39 seed.
        /// passphrase, password, and passwordPrefix are converted to bytes using UTF8 KD normal form encoding.
        /// </summary>
        /// <param name="passphrase">arbitrary passphrase (typically mnemonic words with checksum but not necessarily)</param>
        /// <param name="password">password and passwordPrefix are combined to generate salt bytes.</param>
        /// <param name="passwordPrefix">password and passwordPrefix are combined to generate salt bytes. Default is "mnemonic".</param>
        /// <returns>Computes 512 bit Bip39 seed.</returns>
        public static UInt512 Bip39Seed(string passphrase, string password = null, string passwordPrefix = "mnemonic") {
            return Hashes.PbKdf2HmacSha512(passphrase.Utf8NormalizedToBytes(), $"{passwordPrefix}{password}".Utf8NormalizedToBytes(), 2048);
        }

        /// <summary>
        /// Sets this extended private key per Bip39.
        /// passphrase, password, and passwordPrefix are converted to bytes using UTF8 KD normal form encoding.
        /// </summary>
        /// <param name="passphrase">arbitrary passphrase (typically mnemonic words with checksum but not necessarily)</param>
        /// <param name="password">password and passwordPrefix are combined to generate salt bytes.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="passwordPrefix">password and passwordPrefix are combined to generate salt bytes. Default is "mnemonic".</param>
        /// <returns>Returns this key unless required key paths aren't valid for generated key.</returns>
        public ExtPrivateKey SetMasterBip39(string passphrase, string password = null, IEnumerable<KeyPath> required = null, string passwordPrefix = "mnemonic") {
            var seed = Bip39Seed(passphrase, password, passwordPrefix).ReadOnlySpan;
            return SetMasterBip32(seed, required);
        }

        /// <summary>
        /// Returns a new extended private key to be a master (depth 0) with the given private key and chaincode and verifies required key paths.
        /// </summary>
        /// <param name="privkey">Master private key.</param>
        /// <param name="chaincode">Master chaincode.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <returns>Returns new key unless required key paths aren't valid for specified key in which case null is returned.</returns>
        public static ExtPrivateKey Master(UInt256 privkey, UInt256 chaincode, IEnumerable<KeyPath> required = null)
            => new ExtPrivateKey().SetMaster(privkey, chaincode, required);

        /// <summary>
        /// Returns a new extended private key to be a master (depth 0) with the private key and chaincode set from the single 512 bit vout parameter.
        /// Master private key will be set to the first 256 bits.
        /// Chaincode will be set from the last 256 bits.
        /// </summary>
        /// <param name="vout">Master private key will be set to the first 256 bits. Chaincode will be set from the last 256 bits.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the specified key or returns null.</param>
        /// <returns>Returns new key unless required key paths aren't valid for specified key in which case null is returned.</returns>
        public static ExtPrivateKey Master(UInt512 vout, IEnumerable<KeyPath> required = null)
            => new ExtPrivateKey().SetMaster(vout, required);

        /// <summary>
        /// Returns a new Bip32 private key.
        /// Uses a single invocation of HMACSHA512 to generate 512 bits of entropy with which to set master private key and chaincode.
        /// </summary>
        /// <param name="hmacData">Sequence of bytes passed as hmacData to HMACSHA512 along with byte encoding of hmacKey.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="hmacKey">Default is current global Kz.MasterBip32Key which may default to "Bitcoin seed".</param>
        /// <returns>Returns new key unless required key paths aren't valid for specified key in which case null is returned.</returns>
        public static ExtPrivateKey MasterBip32(ReadOnlySpan<byte> hmacData, IEnumerable<KeyPath> required = null, string hmacKey = null)
            => new ExtPrivateKey().SetMasterBip32(hmacData, required, hmacKey);

        /// <summary>
        /// Returns a new hybrid Bip32 / Bip39 private key.
        /// Uses only a single Bip32 style use of HMACSHA512 starting with at least 32 bytes of Bip39 entropy from mnemonicWords.
        /// </summary>
        /// <param name="mnemonicWords">Must be at least 32 bytes of Bip39 mnemonic word entropy with valid checksum.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="hmacKey">Default is current global Kz.MasterBip32Key which may default to "Bitcoin seed".</param>
        /// <returns>Returns new key unless required key paths aren't valid for specified key in which case null is returned.</returns>
        public static ExtPrivateKey MasterBip32(string mnemonicWords, IEnumerable<KeyPath> required = null, string hmacKey = null)
            => new ExtPrivateKey().SetMasterBip32(mnemonicWords, required, hmacKey);

        /// <summary>
        /// Returns a new extended private key per Bip39.
        /// passphrase, password, and passwordPrefix are converted to bytes using UTF8 KD normal form encoding.
        /// </summary>
        /// <param name="passphrase">arbitrary passphrase (typically mnemonic words with checksum but not necessarily)</param>
        /// <param name="password">password and passwordPrefix are combined to generate salt bytes.</param>
        /// <param name="required">if not null, each key path will be verified as valid on the generated key or returns null.</param>
        /// <param name="passwordPrefix">password and passwordPrefix are combined to generate salt bytes. Default is "mnemonic".</param>
        /// <returns>Returns new key unless required key paths aren't valid for specified key in which case null is returned.</returns>
        public static ExtPrivateKey MasterBip39(string passphrase, string password = null, IEnumerable<KeyPath> required = null, string passwordPrefix = "mnemonic")
            => new ExtPrivateKey().SetMasterBip39(passphrase, password, required, passwordPrefix);
            
        /// <summary>
        /// BIP32 uses "Neuter" to describe adding the extended key information to the public key
        /// associated with an extended private key.
        /// </summary>
        /// <returns></returns>
        public ExtPublicKey GetExtPublicKey() => ExtPublicKey.FromPrivate(this);
        public PublicKey GetPublicKey() => PrivateKey.CreatePublicKey();

        /// <summary>
        /// Computes the private key specified by a key path.
        /// At each derivation, there's a small chance the index specified will fail.
        /// If any generation fails, null is returned.
        /// </summary>
        /// <param name="kp"></param>
        /// <returns>null on derivation failure. Otherwise the derived private key.</returns>
        public ExtPrivateKey Derive(KeyPath kp) => DeriveBase(kp) as ExtPrivateKey;
        public ExtPrivateKey Derive(int index, bool hardened = false) => DeriveBase(index, hardened) as ExtPrivateKey;
        public ExtPrivateKey Derive(uint indexWithHardened) => Derive((int)(indexWithHardened & ~HardenedBit), (indexWithHardened & HardenedBit) != 0);

        public override ExtKey DeriveBase(int index, bool hardened)
        {
            Trace.Assert(index >= 0);
            var cek = new ExtPrivateKey {
                Depth = (byte)(Depth + 1),
                Child = (uint)index | (hardened ? HardenedBit : 0),
                Fingerprint = BitConverter.ToInt32(PrivateKey.CreatePublicKey().GetId().Bytes.Slice(0, 4))
            };

            bool ok;
            (ok, cek.PrivateKey, cek.ChainCode) = PrivateKey.Derive(cek.Child, ChainCode);
            return ok ? cek : null;
        }

        public override void Encode(ByteSpan code)
        {
            code[0] = Depth;
            var s = Fingerprint.AsSpan();
            s.CopyTo(code.Slice(1, 4));
            code[5] = (byte)((Child >> 24) & 0xFF);
            code[6] = (byte)((Child >> 16) & 0xFF);
            code[7] = (byte)((Child >> 8) & 0xFF);
            code[8] = (byte)((Child >> 0) & 0xFF);
            ChainCode.Bytes.CopyTo(code.Slice(9, 32));
            code[41] = 0;
            var key = PrivateKey.ReadOnlySpan;
            Debug.Assert(key.Length == 32);
            key.CopyTo(code.Slice(42, 32));
        }

        public void Decode(ReadOnlySpan<byte> code)
        {
            Depth = code[0];
            code.Slice(1, 4).CopyTo(Fingerprint.AsSpan());
            Child = (uint)code[5] << 24 | (uint)code[6] << 16 | (uint)code[7] << 8 | (uint)(code[8]);
            code.Slice(9, 32).CopyTo(ChainCode.Bytes);
            PrivateKey.Set(code.Slice(42, 32));
        }

        //public KzB58ExtPrivKey ToB58() => new KzB58ExtPrivKey(this);
        //public override string ToString() => ToB58().ToString();

        public override int GetHashCode() => base.GetHashCode() ^ PrivateKey.GetHashCode();
        public bool Equals(ExtPrivateKey o) => (object)o != null && base.Equals(o) && PrivateKey == o.PrivateKey;
        public override bool Equals(object obj) => obj is ExtPrivateKey key && this == key;
        public static bool operator ==(ExtPrivateKey x, ExtPrivateKey y) => x != null && (ReferenceEquals(x, y) || x.Equals(y));
        public static bool operator !=(ExtPrivateKey x, ExtPrivateKey y) => !(x == y);
    }
}
