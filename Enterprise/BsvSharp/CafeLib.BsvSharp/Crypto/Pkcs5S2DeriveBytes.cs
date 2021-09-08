using System;
using System.Collections.Generic;
using CafeLib.BsvSharp.BouncyCastle.Crypto;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Digests;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Macs;
using CafeLib.BsvSharp.BouncyCastle.Crypto.Parameters;

namespace CafeLib.BsvSharp.Crypto
{
    public class Pkcs5S2DeriveBytes
    {
        private readonly IMac _hMac = new HMac(new Sha1Digest());

        private void F(
            byte[] P,
            byte[] S,
            int c,
            byte[] iBuf,
            byte[] outBytes,
            int outOff)
        {
            byte[] state = new byte[_hMac.GetMacSize()];
            ICipherParameters param = new KeyParameter(P);

            _hMac.Init(param);

            if (S != null)
            {
                _hMac.BlockUpdate(S, 0, S.Length);
            }

            _hMac.BlockUpdate(iBuf, 0, iBuf.Length);

            _hMac.DoFinal(state, 0);

            Array.Copy(state, 0, outBytes, outOff, state.Length);

            for (int count = 1; count != c; count++)
            {
                _hMac.Init(param);
                _hMac.BlockUpdate(state, 0, state.Length);
                _hMac.DoFinal(state, 0);

                for (int j = 0; j != state.Length; j++)
                {
                    outBytes[outOff + j] ^= state[j];
                }
            }
        }

        private static void IntToOctet(IList<byte> buffer, int i)
        {
            buffer[0] = (byte)((uint)i >> 24);
            buffer[1] = (byte)((uint)i >> 16);
            buffer[2] = (byte)((uint)i >> 8);
            buffer[3] = (byte)i;
        }

        // Use this function to retrieve a derived key.
        // dkLen is in octets, how much bytes you want when the function to return.
        // mPassword is the password converted to bytes.
        // mSalt is the salt converted to bytes
        // mIterationCount is the how much iterations you want to perform. 


        public byte[] GenerateDerivedKey(
            int dkLen,
            byte[] mPassword,
            byte[] mSalt,
            int mIterationCount
            )
        {
            int hLen = _hMac.GetMacSize();
            int l = (dkLen + hLen - 1) / hLen;
            byte[] iBuf = new byte[4];
            byte[] outBytes = new byte[l * hLen];

            for (int i = 1; i <= l; i++)
            {
                IntToOctet(iBuf, i);

                F(mPassword, mSalt, mIterationCount, iBuf, outBytes, (i - 1) * hLen);
            }

            //By this time outBytes will contain the derived key + more bytes.
            // According to the PKCS #5 v2.0: Password-Based Cryptography Standard (www.truecrypt.org/docs/pkcs5v2-0.pdf) 
            // we have to "extract the first dkLen octets to produce a derived key".

            //I am creating a byte array with the size of dkLen and then using
            //Buffer.BlockCopy to copy ONLY the dkLen amount of bytes to it
            // And finally returning it :D

            byte[] output = new byte[dkLen];

            Buffer.BlockCopy(outBytes, 0, output, 0, dkLen);

            return output;
        }
    }
}
