namespace CafeLib.Cryptography.BouncyCastle.Math.EC.Endo
{
    public interface ECEndomorphism
    {
        ECPointMap PointMap { get; }

        bool HasEfficientPointMap { get; }
    }
}
