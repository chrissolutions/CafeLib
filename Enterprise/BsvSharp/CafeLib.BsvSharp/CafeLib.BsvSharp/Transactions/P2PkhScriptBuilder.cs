using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Transactions
{
    public class P2PkhScriptBuilder : ScriptBuilder
    {
        public P2PkhScriptBuilder(UInt160 pubKeyHash)
            :base(true, TemplateId.Pay2PublicKeyHash)
        {
            Add(Opcode.OP_DUP)
                .Add(Opcode.OP_HASH160)
                .Push(pubKeyHash.Span)
                .Add(Opcode.OP_EQUALVERIFY)
                .Add(Opcode.OP_CHECKSIG);
        }

        public P2PkhScriptBuilder(PublicKey publicKey)
            : this(publicKey.ToPubKeyHash())
        {
        }
    }
}