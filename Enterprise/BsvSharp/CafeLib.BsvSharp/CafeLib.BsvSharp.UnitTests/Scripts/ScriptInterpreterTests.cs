using CafeLib.BsvSharp.Builders;
using CafeLib.BsvSharp.Encoding;
using CafeLib.BsvSharp.Numerics;
using CafeLib.BsvSharp.Scripting;
using CafeLib.BsvSharp.Transactions;
using CafeLib.BsvSharp.Units;
using Xunit;

namespace CafeLib.BsvSharp.UnitTests.Scripts
{
    public class ScriptInterpreterTests
    {
        private static readonly TransactionSignatureChecker DefaultChecker = new TransactionSignatureChecker(new Transaction(), 0, Amount.Zero);

        [Theory]
        [InlineData("OP_1", "OP_1", true)]
        [InlineData("OP_1", "OP_0", false)]
        [InlineData("OP_0", "OP_1", true)]
        [InlineData("OP_CODESEPARATOR", "OP_1", true)]
        [InlineData("", "OP_DEPTH OP_0 OP_EQUAL", true)]
        [InlineData("OP_1 OP_2", "OP_2 OP_EQUALVERIFY OP_1 OP_EQUAL", true)]
        [InlineData("9 0x000000000000000010", "", true)]
        [InlineData("OP_1", "OP_15 OP_ADD OP_16 OP_EQUAL", true)]
        [InlineData("OP_0", "OP_IF OP_VER OP_ELSE OP_1 OP_ENDIF", true)]
        public void VerifyTrivialScriptTest(string scriptSig, string scriptPub, bool result)
        {
            var sig = ScriptBuilder.ParseScript(scriptSig);
            var pub = ScriptBuilder.ParseScript(scriptPub);
            var ok = ScriptInterpreter.VerifyScript(sig, pub, ScriptFlags.VERIFY_NONE, DefaultChecker, out _);
            Assert.Equal(result, ok);
        }

        [Theory]
        [InlineData("0x00", false)]
        [InlineData("0x01", true)]
        [InlineData("0x0080", false)]
        [InlineData("", false)]
        public void VerifyVarType_ToBool(string value, bool result)
        {
            Assert.Equal(new VarType(Encoders.Hex.Decode(value)), result);
        }
    }
}
