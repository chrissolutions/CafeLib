#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Cryptography.UnitTests.BsvSharp.Extensions;

namespace CafeLib.Cryptography.UnitTests.BsvSharp.Keys
{
    public class Base58ExtPrivateKey : Base58Data
    {
        public Base58ExtPrivateKey()
        {
        }

        public Base58ExtPrivateKey(ExtPrivateKey privateKey)
        {
            SetKey(privateKey);
        }

        public Base58ExtPrivateKey(string b58)
        {
            SetString(b58);
        }

        public void SetKey(ExtPrivateKey privateKey)
        {
            var prefix = UnitTest.Network.ExtSecretKey;
            var data = new byte[prefix.Length + ExtKey.Bip32KeySize];
            prefix.CopyTo(data, 0);
            privateKey.Encode(data.Slice(prefix.Length));
            SetData(data, prefix.Length);
        }

        public ExtPrivateKey GetKey()
        {
            var privateKey = new ExtPrivateKey();
            if (KeyData.Length == ExtKey.Bip32KeySize)
            {
                privateKey.Decode(KeyData);
            }
            return privateKey;
        }

        public bool SetString(string b58) => SetString(b58, UnitTest.Network.ExtSecretKey.Length);
    }
}
