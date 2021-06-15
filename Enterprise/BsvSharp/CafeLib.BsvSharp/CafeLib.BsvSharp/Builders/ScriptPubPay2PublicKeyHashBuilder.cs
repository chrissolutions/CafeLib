using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    public class ScriptPubPay2PublicKeyHashBuilder : ScriptBuilder
    {
        public ScriptPubPay2PublicKeyHashBuilder(UInt160 pubKeyHash)
        {
            IsPub = true;
            _TemplateId = TemplateId.Pay2PublicKeyHash;
            Add(Opcode.OP_DUP)
                .Add(Opcode.OP_HASH160)
                .Push(pubKeyHash.Span)
                .Add(Opcode.OP_EQUALVERIFY)
                .Add(Opcode.OP_CHECKSIG)
                ;
        }

        public ScriptPubPay2PublicKeyHashBuilder(PublicKey publicKey)
            : this(publicKey.ToHash160())

        {
        }
    }
}