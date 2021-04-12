﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Shared.Extensions;
using CafeLib.Bitcoin.Shared.Services;

namespace CafeLib.Bitcoin.Shared.Keys
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
            var prefix = RootService.Network.ExtSecretKey;
            var data = new byte[prefix.Length + ExtKey.Bip32KeySize];
            prefix.CopyTo(data.Slice(0, prefix.Length));
            privateKey.Encode(data.Slice(prefix.Length));
            SetData(data, prefix.Length);
        }

        public ExtPrivateKey GetKey()
        {
            var privKey = new ExtPrivateKey();
            if (Data.Length == ExtKey.Bip32KeySize)
            {
                privKey.Decode(Data);
            }
            return privKey;
        }

        public bool SetString(string b58) => base.SetString(b58, RootService.Network.ExtSecretKey.Length);
    }
}
