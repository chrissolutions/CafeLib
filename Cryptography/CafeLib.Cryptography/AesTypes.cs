namespace CafeLib.Cryptography
{
    internal static class AesTypes
    {
        public enum CipherMode
        {
            CBC,
            GCM
        }

        public enum Padding
        {
            NoPadding,
            PKCS7
        }
    }
}