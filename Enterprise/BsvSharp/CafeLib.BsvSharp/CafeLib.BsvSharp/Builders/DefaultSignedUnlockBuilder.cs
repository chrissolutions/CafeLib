using System.Collections.Generic;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Signatures;

namespace CafeLib.BsvSharp.Builders
{
    public class DefaultSignedUnlockBuilder : SignedUnlockBuilder
    {
        internal DefaultSignedUnlockBuilder()
            : this(null)
        {
        }

        public DefaultSignedUnlockBuilder(PublicKey pubKey, TemplateId templateId = TemplateId.Unknown)
            : base(pubKey, templateId)
        {
        }
    }
}