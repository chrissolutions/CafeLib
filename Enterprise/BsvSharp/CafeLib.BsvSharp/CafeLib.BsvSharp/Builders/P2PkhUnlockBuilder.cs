#region Copyright
// Copyright (c) 2020 TonesNotes
// Distributed under the Open BSV software license, see the accompanying file LICENSE.
#endregion

using System.Linq;
using CafeLib.BsvSharp.Exceptions;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Signatures;
using CafeLib.Core.Buffers.Arrays;

namespace CafeLib.BsvSharp.Builders
{
    public class P2PkhUnlockBuilder : SignedUnlockBuilder
    {
        private readonly ArrayBuffer<Signature> _signatures = new ArrayBuffer<Signature>();

        /// <summary>
        /// P2PkhUnlockBuilder constructor.
        /// </summary>
        /// <param name="publicKey">public key</param>
        public P2PkhUnlockBuilder(PublicKey publicKey)
            : base(publicKey, TemplateId.Pay2PublicKeyHash)
        {
            Signatures = _signatures;
            Push(new byte[72]) // This will become the CHECKSIG signature
                .Push(publicKey);
        }
        
        public P2PkhUnlockBuilder(Script script)
            : base(null, TemplateId.Pay2PublicKeyHash)
        {
            Signatures = _signatures;
            UnlockScript(script);
        }

        public override void AddSignature(Signature signature)
        {
            _signatures.Add(signature);
        }

        public override void Sign(Script scriptSig)
        {
            UnlockScript(scriptSig);
        }

        private void UnlockScript(Script scriptSig)
        {
            if (scriptSig == Script.None)
            {
                throw new ScriptException("Invalid Script or Malformed Script.");
            }

            Set(scriptSig);

            if (Ops.Count != 2)
            {
                throw new ScriptException("Wrong number of data elements for P2PKH ScriptSig");
            }

            _signatures.Add(new Signature(Ops.First().Operand.Data));
            PublicKey = new PublicKey(Ops.Last().Operand.Data);
        }
    }
}
