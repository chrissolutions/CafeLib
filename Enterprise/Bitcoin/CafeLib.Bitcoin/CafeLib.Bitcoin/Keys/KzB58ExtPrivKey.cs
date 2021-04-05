#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Extensions;
using CafeLib.Bitcoin.Global;

namespace CafeLib.Bitcoin.Keys
{
    public class KzB58ExtPrivKey : KzB58Data
    {
        public void SetKey(KzExtPrivKey privKey)
        {
            var prefix = Kz.ExtSecretKey;
            var data = new byte[prefix.Length + KzExtKey.BIP32_EXTKEY_SIZE];
            prefix.CopyTo(data.Slice(0, prefix.Length));
            privKey.Encode(data.Slice(prefix.Length));
            SetData(data, prefix.Length);
        }

        public KzExtPrivKey GetKey()
        {
            var privKey = new KzExtPrivKey();
            if (Data.Length == KzExtKey.BIP32_EXTKEY_SIZE) {
                privKey.Decode(Data);
            }
            return privKey;
        }

        public bool SetString(string b58) => base.SetString(b58, Kz.ExtSecretKey.Length);

        public KzB58ExtPrivKey() { }
        public KzB58ExtPrivKey(KzExtPrivKey privKey) { SetKey(privKey); }
        public KzB58ExtPrivKey(string b58) { SetString(b58); }
    }
}
