using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    public class SignedUnlockBuilder : ScriptBuilder
    {
        public PublicKey PublicKey { get; }

        internal SignedUnlockBuilder()
            : this(null, TemplateId.Unknown)
        {
        }
        
        public SignedUnlockBuilder(PublicKey pubKey, TemplateId templateId = TemplateId.Unknown)
            : base(false, templateId)
        {
            PublicKey = pubKey;
        }
    }
}