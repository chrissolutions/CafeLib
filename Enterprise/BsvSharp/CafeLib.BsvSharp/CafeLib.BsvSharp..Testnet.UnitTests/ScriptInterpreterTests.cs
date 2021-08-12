using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Keys;
using CafeLib.BsvSharp.Network;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Services;
using CafeLib.BsvSharp.Signatures;
using CafeLib.BsvSharp.Transactions;
using Xunit;

namespace CafeLib.BsvSharp.Testnet.UnitTests
{
    public class ScriptInterpreterTests
    {
        public ScriptInterpreterTests()
        {
            RootService.Bootstrap(NetworkType.Test);
        }

        [Fact]
        public void Verify_Script_From_Simple_Transaction()
        {
            var privateKey = PrivateKey.FromWif("cSBnVM4xvxarwGQuAfQFwqDg9k5tErHUHzgWsEfD4zdwUasvqRVY");
            var publicKey = privateKey.CreatePublicKey();
            var fromAddress = publicKey.ToAddress();
            var toAddress = new Address("mrU9pEmAx26HcbKVrABvgL7AwA5fjNFoDc");

            var utxo = new Utxo
            {
                TxHash = new UInt256("a477af6b2667c29670467e4e0728b685ee07b240235771862318e29ddbe58458"),
                Index = 0,
                ScriptPubKey = new P2PkhLockBuilder(fromAddress).ToScript(),
                Amount = 100000
            };

            var tx = new Transaction();
            tx.SpendFromUtxo(utxo, new P2PkhUnlockBuilder(publicKey));
            tx.SpendTo(toAddress, 100000L, new P2PkhLockBuilder(toAddress));
            tx.SignInput(0, privateKey, SignatureHashEnum.All);

            // we then extract the signature from the first input
            var scriptSig = tx.Inputs[0].ScriptSig;
            
            const ScriptFlags flags = ScriptFlags.VERIFY_P2SH | ScriptFlags.VERIFY_STRICTENC;
            var checker = new TransactionSignatureChecker(tx, 0, utxo.Amount);
            var verified = ScriptInterpreter.VerifyScript(scriptSig, utxo.ScriptPubKey, flags, checker, out var _);
            Assert.True(verified);
        }
    }
}
