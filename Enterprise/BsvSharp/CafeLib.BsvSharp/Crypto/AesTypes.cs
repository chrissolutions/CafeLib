using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.BsvSharp.Crypto
{
    public static class AesTypes
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
