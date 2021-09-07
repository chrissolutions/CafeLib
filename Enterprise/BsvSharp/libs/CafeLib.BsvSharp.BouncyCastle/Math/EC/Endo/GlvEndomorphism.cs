namespace CafeLib.BsvSharp.BouncyCastle.Math.EC.Endo
{
    public interface GlvEndomorphism
        :   ECEndomorphism
    {
        BigInteger[] DecomposeScalar(BigInteger k);
    }
}
