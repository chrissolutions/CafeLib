﻿#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    public class ScriptSigPay2PublicKeyHashBuilder : ScriptBuilder
    {
        public ScriptSigPay2PublicKeyHashBuilder(PublicKey pubKey)
            : base(false, TemplateId.Pay2PublicKeyHash)
        {
            Push(new byte[72]) // This will become the CHECKSIG signature
                .Push(pubKey);
        }
    }
}
