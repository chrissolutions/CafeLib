#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Transactions
{
    public class SigP2PkhScriptBuilder : ScriptBuilder
    {
        public SigP2PkhScriptBuilder(PublicKey pubKey)
            : base(false, TemplateId.Pay2PublicKeyHash)
        {
            Push(new byte[72]) // This will become the CHECKSIG signature
                .Push(pubKey);
        }
    }
}
