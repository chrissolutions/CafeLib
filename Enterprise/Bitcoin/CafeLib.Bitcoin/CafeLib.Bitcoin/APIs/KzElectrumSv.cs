#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Keys;

namespace CafeLib.Bitcoin.APIs {

    public class KzElectrumSv
    {
        public static KzExtPrivKey GetMasterPrivKey(string passphrase, string password = null) =>
            KzExtPrivKey.MasterBip39(passphrase, password, passwordPrefix: "electrum");
    }
}
