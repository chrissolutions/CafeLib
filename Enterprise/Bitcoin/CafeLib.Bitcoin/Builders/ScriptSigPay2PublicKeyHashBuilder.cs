#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.Bitcoin.Keys;
using CafeLib.Bitcoin.Scripting;

namespace CafeLib.Bitcoin.Builders
{
    public class ScriptSigPay2PublicKeyHashBuilder : ScriptBuilder
    {
        public ScriptSigPay2PublicKeyHashBuilder(PublicKey pubKey)
        {
            IsPub = false;
            _TemplateId = TemplateId.Pay2PublicKeyHash;
            this
                .Push(new byte[72]) // This will become the CHECKSIG signature
                .Push(pubKey.Bytes)
                ;
        }
    }
}
