using System.Collections.Generic;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Signatures;

namespace CafeLib.BsvSharp.Builders
{
    public class DefaultSignedUnlockBuilder : SignedUnlockBuilder
    {
        private readonly List<Signature> _signatures = new List<Signature>();

        internal DefaultSignedUnlockBuilder()
            : this(null)
        {
            Signatures = _signatures;
        }

        protected DefaultSignedUnlockBuilder(PublicKey pubKey, TemplateId templateId = TemplateId.Unknown)
            : base(pubKey, templateId)
        {
            Signatures = _signatures;
        }
    }
}