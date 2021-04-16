﻿using CafeLib.Bitcoin.Shared.Numerics;
using CafeLib.Bitcoin.Shared.Scripting;

namespace CafeLib.Bitcoin.Shared.Builders
{
    public class ScriptPubPay2PublicKeyHashBuilder : ScriptBuilder
    {
        public ScriptPubPay2PublicKeyHashBuilder(UInt160 pubKeyHash)
        {
            IsPub = true;
            _TemplateId = TemplateId.Pay2PublicKeyHash;
            Add(Opcode.OP_DUP)
                .Add(Opcode.OP_HASH160)
                .Push(pubKeyHash.Bytes)
                .Add(Opcode.OP_EQUALVERIFY)
                .Add(Opcode.OP_CHECKSIG)
                ;
        }
    }
}