#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System;
using System.Diagnostics;
using CafeLib.Bitcoin.Buffers;
using CafeLib.Bitcoin.Services;

namespace CafeLib.Bitcoin.Keys
{
    public class Base58PrivateKey : Base58Data
    {
        public void SetKey(PrivateKey privateKey)
        {
            Debug.Assert(privateKey.IsValid);
            SetData(RootService.Network.SecretKey, privateKey.Bytes, privateKey.IsCompressed);
        }

        public PrivateKey GetKey()
        {
            var data = Data;
            Debug.Assert(data.Length >= 32);
            var isCompressed = data.Length > 32 && data[32] == 1;
            var privateKey = new PrivateKey((ByteSpan)data.Slice(0, 32), isCompressed);
            return privateKey;
        }

        public bool IsValid
        {
            get 
            {
                var d = Data;
                var fExpectedFormat = d.Length == 32 || d.Length == 33 && d[^1] == 1;
                var v = Version;
                var fCorrectVersion = v.Data.SequenceEqual(RootService.Network.SecretKey);
                return fExpectedFormat && fCorrectVersion;
            }
        }

        public bool SetString(string base58) => SetString(base58, RootService.Network.SecretKey.Length) && IsValid;

        public Base58PrivateKey() {}
        public Base58PrivateKey(PrivateKey privateKey) => SetKey(privateKey);
        public Base58PrivateKey(string base58) => SetString(base58);
    }
}
