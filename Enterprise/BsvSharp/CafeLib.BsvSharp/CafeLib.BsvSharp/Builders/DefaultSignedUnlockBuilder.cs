using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    public class DefaultSignedUnlockBuilder : SignedUnlockBuilder
    {
        internal DefaultSignedUnlockBuilder()
            : this(null)
        {
        }

        protected DefaultSignedUnlockBuilder(PublicKey pubKey, TemplateId templateId = TemplateId.Unknown)
            : base(pubKey, templateId)
        {
        }

        //public virtual void Sign(Script scriptSig)
        //{
        //    Set(scriptSig);
        //}

        //public override Script ToScript()
        //{
        //    if (!Signatures.Any())
        //    {
        //        return Ops.Any() ? base.ToScript() : Script.None;
        //    }

        //    base.Clear();
        //    Push(Signatures.First().Data)
        //        .Push(PublicKey);

        //    return base.ToScript();
        //}
    }
}