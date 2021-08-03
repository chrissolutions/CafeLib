using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;

namespace CafeLib.BsvSharp.Builders
{
    public class P2PkhLockBuilder : ScriptBuilder
    {
        public P2PkhLockBuilder(Address address)
            : this(address.PubKeyHash)
        {
        }

        public P2PkhLockBuilder(PublicKey publicKey)
            : this(publicKey.ToPubKeyHash())
        {
        }

        protected P2PkhLockBuilder(UInt160 pubKeyHash)
            :base(true, TemplateId.Pay2PublicKeyHash)
        {
            Add(Opcode.OP_DUP)
                .Add(Opcode.OP_HASH160)
                .Push(pubKeyHash.Span)
                .Add(Opcode.OP_EQUALVERIFY)
                .Add(Opcode.OP_CHECKSIG);
        }
    }
}