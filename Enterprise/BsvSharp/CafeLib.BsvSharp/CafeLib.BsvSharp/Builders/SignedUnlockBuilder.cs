﻿using System;
using System.Collections.Generic;
using System.Linq;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Signatures;

namespace CafeLib.BsvSharp.Builders
{
    public class SignedUnlockBuilder : ScriptBuilder
    {
        public PublicKey PublicKey { get; protected set; }

        public IEnumerable<Signature> Signatures { get; protected set; } = ArraySegment<Signature>.Empty;

        public virtual void AddSignature(Signature signature) => 
            (Signatures as ICollection<Signature>)?.Add(signature);

        internal SignedUnlockBuilder()
            : this(null)
        {
        }

        protected SignedUnlockBuilder(PublicKey pubKey, TemplateId templateId = TemplateId.Unknown)
            : base(false, templateId)
        {
            PublicKey = pubKey;
        }

        public virtual void Sign(Script scriptSig)
        {
            Set(scriptSig);
        }

        public override Script ToScript()
        {
            if (!Signatures.Any())
            {
                return Ops.Any() ? base.ToScript() : Script.None;
            }

            base.Clear();
            Push(Signatures.First().Data)
                .Push(PublicKey);

            return base.ToScript();
        }
    }
}