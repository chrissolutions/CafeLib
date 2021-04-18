using System;

namespace CafeLib.Bitcoin.Crypto
{
    /**
    * implementation of RipeMD see,
    * http://www.esat.kuleuven.ac.be/~bosselae/ripemd160.html
    */
    internal class Ripemd160Digest : GeneralDigest
    {
        private const int DigestLength = 20;

        private int _h0, _h1, _h2, _h3, _h4; // IV's
        private readonly int[] _x = new int[16];
        private int _xOff;

        /**
        * Standard constructor
        */
        public Ripemd160Digest()
        {
            Clear();
        }

        /**
        * Copy constructor.  This will copy the state of the provided
        * message digest.
        */
        public Ripemd160Digest(Ripemd160Digest t) : base(t)
        {
            CopyIn(t);
        }

        private void CopyIn(Ripemd160Digest t)
        {
            base.CopyIn(t);

            _h0 = t._h0;
            _h1 = t._h1;
            _h2 = t._h2;
            _h3 = t._h3;
            _h4 = t._h4;

            Array.Copy(t._x, 0, _x, 0, t._x.Length);
            _xOff = t._xOff;
        }

        public override string AlgorithmName
        {
            get
            {
                return "RIPEMD160";
            }
        }

        public override int GetDigestSize()
        {
            return DigestLength;
        }

        internal override void ProcessWord(
            ReadOnlySpan<byte> input,
            int inOff)
        {
            _x[_xOff++] = (input[inOff] & 0xff) | ((input[inOff + 1] & 0xff) << 8)
                | ((input[inOff + 2] & 0xff) << 16) | ((input[inOff + 3] & 0xff) << 24);

            if (_xOff == 16)
            {
                ProcessBlock();
            }
        }

        internal override void ProcessLength(
            long bitLength)
        {
            if (_xOff > 14)
            {
                ProcessBlock();
            }

            _x[14] = (int)(bitLength & 0xffffffff);
            _x[15] = (int)((ulong)bitLength >> 32);
        }

        private void UnpackWord(int word, Span<byte> outBytes, int outOff)
        {
            outBytes[outOff] = (byte)word;
            outBytes[outOff + 1] = (byte)((uint)word >> 8);
            outBytes[outOff + 2] = (byte)((uint)word >> 16);
            outBytes[outOff + 3] = (byte)((uint)word >> 24);
        }

        public override int DoFinal(Span<byte> output)
        {
            Finish();

            UnpackWord(_h0, output, 0);
            UnpackWord(_h1, output, 4);
            UnpackWord(_h2, output, 8);
            UnpackWord(_h3, output, 12);
            UnpackWord(_h4, output, 16);

            Reset();

            return DigestLength;
        }

        /**
        * reset the chaining variables to the IV values.
        */
        public override void Reset()
        {
            base.Reset();
            Clear();
        }

        public void Clear()
        {
            _h0 = unchecked((int)0x67452301);
            _h1 = unchecked((int)0xefcdab89);
            _h2 = unchecked((int)0x98badcfe);
            _h3 = unchecked((int)0x10325476);
            _h4 = unchecked((int)0xc3d2e1f0);

            _xOff = 0;

            for (int i = 0; i != _x.Length; i++)
            {
                _x[i] = 0;
            }
        }

        /*
        * rotate int x left n bits.
        */
        private int RL(
            int x,
            int n)
        {
            return (x << n) | (int)((uint)x >> (32 - n));
        }

        /*
        * f1,f2,f3,f4,f5 are the basic RipeMD160 functions.
        */

        /*
        * rounds 0-15
        */
        private int F1(
            int x,
            int y,
            int z)
        {
            return x ^ y ^ z;
        }

        /*
        * rounds 16-31
        */
        private int F2(
            int x,
            int y,
            int z)
        {
            return (x & y) | (~x & z);
        }

        /*
        * rounds 32-47
        */
        private int F3(
            int x,
            int y,
            int z)
        {
            return (x | ~y) ^ z;
        }

        /*
        * rounds 48-63
        */
        private int F4(
            int x,
            int y,
            int z)
        {
            return (x & z) | (y & ~z);
        }

        /*
        * rounds 64-79
        */
        private static int F5(
            int x,
            int y,
            int z)
        {
            return x ^ (y | ~z);
        }

        internal override void ProcessBlock()
        {
            int a, aa;
            int b, bb;
            int c, cc;
            int d, dd;
            int e, ee;

            a = aa = _h0;
            b = bb = _h1;
            c = cc = _h2;
            d = dd = _h3;
            e = ee = _h4;

            //
            // Rounds 1 - 16
            //
            // left
            a = RL(a + F1(b, c, d) + _x[0], 11) + e;
            c = RL(c, 10);
            e = RL(e + F1(a, b, c) + _x[1], 14) + d;
            b = RL(b, 10);
            d = RL(d + F1(e, a, b) + _x[2], 15) + c;
            a = RL(a, 10);
            c = RL(c + F1(d, e, a) + _x[3], 12) + b;
            e = RL(e, 10);
            b = RL(b + F1(c, d, e) + _x[4], 5) + a;
            d = RL(d, 10);
            a = RL(a + F1(b, c, d) + _x[5], 8) + e;
            c = RL(c, 10);
            e = RL(e + F1(a, b, c) + _x[6], 7) + d;
            b = RL(b, 10);
            d = RL(d + F1(e, a, b) + _x[7], 9) + c;
            a = RL(a, 10);
            c = RL(c + F1(d, e, a) + _x[8], 11) + b;
            e = RL(e, 10);
            b = RL(b + F1(c, d, e) + _x[9], 13) + a;
            d = RL(d, 10);
            a = RL(a + F1(b, c, d) + _x[10], 14) + e;
            c = RL(c, 10);
            e = RL(e + F1(a, b, c) + _x[11], 15) + d;
            b = RL(b, 10);
            d = RL(d + F1(e, a, b) + _x[12], 6) + c;
            a = RL(a, 10);
            c = RL(c + F1(d, e, a) + _x[13], 7) + b;
            e = RL(e, 10);
            b = RL(b + F1(c, d, e) + _x[14], 9) + a;
            d = RL(d, 10);
            a = RL(a + F1(b, c, d) + _x[15], 8) + e;
            c = RL(c, 10);

            // right
            aa = RL(aa + F5(bb, cc, dd) + _x[5] + unchecked((int)0x50a28be6), 8) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F5(aa, bb, cc) + _x[14] + unchecked((int)0x50a28be6), 9) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F5(ee, aa, bb) + _x[7] + unchecked((int)0x50a28be6), 9) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F5(dd, ee, aa) + _x[0] + unchecked((int)0x50a28be6), 11) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F5(cc, dd, ee) + _x[9] + unchecked((int)0x50a28be6), 13) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F5(bb, cc, dd) + _x[2] + unchecked((int)0x50a28be6), 15) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F5(aa, bb, cc) + _x[11] + unchecked((int)0x50a28be6), 15) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F5(ee, aa, bb) + _x[4] + unchecked((int)0x50a28be6), 5) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F5(dd, ee, aa) + _x[13] + unchecked((int)0x50a28be6), 7) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F5(cc, dd, ee) + _x[6] + unchecked((int)0x50a28be6), 7) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F5(bb, cc, dd) + _x[15] + unchecked((int)0x50a28be6), 8) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F5(aa, bb, cc) + _x[8] + unchecked((int)0x50a28be6), 11) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F5(ee, aa, bb) + _x[1] + unchecked((int)0x50a28be6), 14) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F5(dd, ee, aa) + _x[10] + unchecked((int)0x50a28be6), 14) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F5(cc, dd, ee) + _x[3] + unchecked((int)0x50a28be6), 12) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F5(bb, cc, dd) + _x[12] + unchecked((int)0x50a28be6), 6) + ee;
            cc = RL(cc, 10);

            //
            // Rounds 16-31
            //
            // left
            e = RL(e + F2(a, b, c) + _x[7] + unchecked((int)0x5a827999), 7) + d;
            b = RL(b, 10);
            d = RL(d + F2(e, a, b) + _x[4] + unchecked((int)0x5a827999), 6) + c;
            a = RL(a, 10);
            c = RL(c + F2(d, e, a) + _x[13] + unchecked((int)0x5a827999), 8) + b;
            e = RL(e, 10);
            b = RL(b + F2(c, d, e) + _x[1] + unchecked((int)0x5a827999), 13) + a;
            d = RL(d, 10);
            a = RL(a + F2(b, c, d) + _x[10] + unchecked((int)0x5a827999), 11) + e;
            c = RL(c, 10);
            e = RL(e + F2(a, b, c) + _x[6] + unchecked((int)0x5a827999), 9) + d;
            b = RL(b, 10);
            d = RL(d + F2(e, a, b) + _x[15] + unchecked((int)0x5a827999), 7) + c;
            a = RL(a, 10);
            c = RL(c + F2(d, e, a) + _x[3] + unchecked((int)0x5a827999), 15) + b;
            e = RL(e, 10);
            b = RL(b + F2(c, d, e) + _x[12] + unchecked((int)0x5a827999), 7) + a;
            d = RL(d, 10);
            a = RL(a + F2(b, c, d) + _x[0] + unchecked((int)0x5a827999), 12) + e;
            c = RL(c, 10);
            e = RL(e + F2(a, b, c) + _x[9] + unchecked((int)0x5a827999), 15) + d;
            b = RL(b, 10);
            d = RL(d + F2(e, a, b) + _x[5] + unchecked((int)0x5a827999), 9) + c;
            a = RL(a, 10);
            c = RL(c + F2(d, e, a) + _x[2] + unchecked((int)0x5a827999), 11) + b;
            e = RL(e, 10);
            b = RL(b + F2(c, d, e) + _x[14] + unchecked((int)0x5a827999), 7) + a;
            d = RL(d, 10);
            a = RL(a + F2(b, c, d) + _x[11] + unchecked((int)0x5a827999), 13) + e;
            c = RL(c, 10);
            e = RL(e + F2(a, b, c) + _x[8] + unchecked((int)0x5a827999), 12) + d;
            b = RL(b, 10);

            // right
            ee = RL(ee + F4(aa, bb, cc) + _x[6] + unchecked((int)0x5c4dd124), 9) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F4(ee, aa, bb) + _x[11] + unchecked((int)0x5c4dd124), 13) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F4(dd, ee, aa) + _x[3] + unchecked((int)0x5c4dd124), 15) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F4(cc, dd, ee) + _x[7] + unchecked((int)0x5c4dd124), 7) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F4(bb, cc, dd) + _x[0] + unchecked((int)0x5c4dd124), 12) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F4(aa, bb, cc) + _x[13] + unchecked((int)0x5c4dd124), 8) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F4(ee, aa, bb) + _x[5] + unchecked((int)0x5c4dd124), 9) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F4(dd, ee, aa) + _x[10] + unchecked((int)0x5c4dd124), 11) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F4(cc, dd, ee) + _x[14] + unchecked((int)0x5c4dd124), 7) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F4(bb, cc, dd) + _x[15] + unchecked((int)0x5c4dd124), 7) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F4(aa, bb, cc) + _x[8] + unchecked((int)0x5c4dd124), 12) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F4(ee, aa, bb) + _x[12] + unchecked((int)0x5c4dd124), 7) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F4(dd, ee, aa) + _x[4] + unchecked((int)0x5c4dd124), 6) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F4(cc, dd, ee) + _x[9] + unchecked((int)0x5c4dd124), 15) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F4(bb, cc, dd) + _x[1] + unchecked((int)0x5c4dd124), 13) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F4(aa, bb, cc) + _x[2] + unchecked((int)0x5c4dd124), 11) + dd;
            bb = RL(bb, 10);

            //
            // Rounds 32-47
            //
            // left
            d = RL(d + F3(e, a, b) + _x[3] + unchecked((int)0x6ed9eba1), 11) + c;
            a = RL(a, 10);
            c = RL(c + F3(d, e, a) + _x[10] + unchecked((int)0x6ed9eba1), 13) + b;
            e = RL(e, 10);
            b = RL(b + F3(c, d, e) + _x[14] + unchecked((int)0x6ed9eba1), 6) + a;
            d = RL(d, 10);
            a = RL(a + F3(b, c, d) + _x[4] + unchecked((int)0x6ed9eba1), 7) + e;
            c = RL(c, 10);
            e = RL(e + F3(a, b, c) + _x[9] + unchecked((int)0x6ed9eba1), 14) + d;
            b = RL(b, 10);
            d = RL(d + F3(e, a, b) + _x[15] + unchecked((int)0x6ed9eba1), 9) + c;
            a = RL(a, 10);
            c = RL(c + F3(d, e, a) + _x[8] + unchecked((int)0x6ed9eba1), 13) + b;
            e = RL(e, 10);
            b = RL(b + F3(c, d, e) + _x[1] + unchecked((int)0x6ed9eba1), 15) + a;
            d = RL(d, 10);
            a = RL(a + F3(b, c, d) + _x[2] + unchecked((int)0x6ed9eba1), 14) + e;
            c = RL(c, 10);
            e = RL(e + F3(a, b, c) + _x[7] + unchecked((int)0x6ed9eba1), 8) + d;
            b = RL(b, 10);
            d = RL(d + F3(e, a, b) + _x[0] + unchecked((int)0x6ed9eba1), 13) + c;
            a = RL(a, 10);
            c = RL(c + F3(d, e, a) + _x[6] + unchecked((int)0x6ed9eba1), 6) + b;
            e = RL(e, 10);
            b = RL(b + F3(c, d, e) + _x[13] + unchecked((int)0x6ed9eba1), 5) + a;
            d = RL(d, 10);
            a = RL(a + F3(b, c, d) + _x[11] + unchecked((int)0x6ed9eba1), 12) + e;
            c = RL(c, 10);
            e = RL(e + F3(a, b, c) + _x[5] + unchecked((int)0x6ed9eba1), 7) + d;
            b = RL(b, 10);
            d = RL(d + F3(e, a, b) + _x[12] + unchecked((int)0x6ed9eba1), 5) + c;
            a = RL(a, 10);

            // right
            dd = RL(dd + F3(ee, aa, bb) + _x[15] + unchecked((int)0x6d703ef3), 9) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F3(dd, ee, aa) + _x[5] + unchecked((int)0x6d703ef3), 7) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F3(cc, dd, ee) + _x[1] + unchecked((int)0x6d703ef3), 15) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F3(bb, cc, dd) + _x[3] + unchecked((int)0x6d703ef3), 11) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F3(aa, bb, cc) + _x[7] + unchecked((int)0x6d703ef3), 8) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F3(ee, aa, bb) + _x[14] + unchecked((int)0x6d703ef3), 6) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F3(dd, ee, aa) + _x[6] + unchecked((int)0x6d703ef3), 6) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F3(cc, dd, ee) + _x[9] + unchecked((int)0x6d703ef3), 14) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F3(bb, cc, dd) + _x[11] + unchecked((int)0x6d703ef3), 12) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F3(aa, bb, cc) + _x[8] + unchecked((int)0x6d703ef3), 13) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F3(ee, aa, bb) + _x[12] + unchecked((int)0x6d703ef3), 5) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F3(dd, ee, aa) + _x[2] + unchecked((int)0x6d703ef3), 14) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F3(cc, dd, ee) + _x[10] + unchecked((int)0x6d703ef3), 13) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F3(bb, cc, dd) + _x[0] + unchecked((int)0x6d703ef3), 13) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F3(aa, bb, cc) + _x[4] + unchecked((int)0x6d703ef3), 7) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F3(ee, aa, bb) + _x[13] + unchecked((int)0x6d703ef3), 5) + cc;
            aa = RL(aa, 10);

            //
            // Rounds 48-63
            //
            // left
            c = RL(c + F4(d, e, a) + _x[1] + unchecked((int)0x8f1bbcdc), 11) + b;
            e = RL(e, 10);
            b = RL(b + F4(c, d, e) + _x[9] + unchecked((int)0x8f1bbcdc), 12) + a;
            d = RL(d, 10);
            a = RL(a + F4(b, c, d) + _x[11] + unchecked((int)0x8f1bbcdc), 14) + e;
            c = RL(c, 10);
            e = RL(e + F4(a, b, c) + _x[10] + unchecked((int)0x8f1bbcdc), 15) + d;
            b = RL(b, 10);
            d = RL(d + F4(e, a, b) + _x[0] + unchecked((int)0x8f1bbcdc), 14) + c;
            a = RL(a, 10);
            c = RL(c + F4(d, e, a) + _x[8] + unchecked((int)0x8f1bbcdc), 15) + b;
            e = RL(e, 10);
            b = RL(b + F4(c, d, e) + _x[12] + unchecked((int)0x8f1bbcdc), 9) + a;
            d = RL(d, 10);
            a = RL(a + F4(b, c, d) + _x[4] + unchecked((int)0x8f1bbcdc), 8) + e;
            c = RL(c, 10);
            e = RL(e + F4(a, b, c) + _x[13] + unchecked((int)0x8f1bbcdc), 9) + d;
            b = RL(b, 10);
            d = RL(d + F4(e, a, b) + _x[3] + unchecked((int)0x8f1bbcdc), 14) + c;
            a = RL(a, 10);
            c = RL(c + F4(d, e, a) + _x[7] + unchecked((int)0x8f1bbcdc), 5) + b;
            e = RL(e, 10);
            b = RL(b + F4(c, d, e) + _x[15] + unchecked((int)0x8f1bbcdc), 6) + a;
            d = RL(d, 10);
            a = RL(a + F4(b, c, d) + _x[14] + unchecked((int)0x8f1bbcdc), 8) + e;
            c = RL(c, 10);
            e = RL(e + F4(a, b, c) + _x[5] + unchecked((int)0x8f1bbcdc), 6) + d;
            b = RL(b, 10);
            d = RL(d + F4(e, a, b) + _x[6] + unchecked((int)0x8f1bbcdc), 5) + c;
            a = RL(a, 10);
            c = RL(c + F4(d, e, a) + _x[2] + unchecked((int)0x8f1bbcdc), 12) + b;
            e = RL(e, 10);

            // right
            cc = RL(cc + F2(dd, ee, aa) + _x[8] + unchecked((int)0x7a6d76e9), 15) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F2(cc, dd, ee) + _x[6] + unchecked((int)0x7a6d76e9), 5) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F2(bb, cc, dd) + _x[4] + unchecked((int)0x7a6d76e9), 8) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F2(aa, bb, cc) + _x[1] + unchecked((int)0x7a6d76e9), 11) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F2(ee, aa, bb) + _x[3] + unchecked((int)0x7a6d76e9), 14) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F2(dd, ee, aa) + _x[11] + unchecked((int)0x7a6d76e9), 14) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F2(cc, dd, ee) + _x[15] + unchecked((int)0x7a6d76e9), 6) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F2(bb, cc, dd) + _x[0] + unchecked((int)0x7a6d76e9), 14) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F2(aa, bb, cc) + _x[5] + unchecked((int)0x7a6d76e9), 6) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F2(ee, aa, bb) + _x[12] + unchecked((int)0x7a6d76e9), 9) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F2(dd, ee, aa) + _x[2] + unchecked((int)0x7a6d76e9), 12) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F2(cc, dd, ee) + _x[13] + unchecked((int)0x7a6d76e9), 9) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F2(bb, cc, dd) + _x[9] + unchecked((int)0x7a6d76e9), 12) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F2(aa, bb, cc) + _x[7] + unchecked((int)0x7a6d76e9), 5) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F2(ee, aa, bb) + _x[10] + unchecked((int)0x7a6d76e9), 15) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F2(dd, ee, aa) + _x[14] + unchecked((int)0x7a6d76e9), 8) + bb;
            ee = RL(ee, 10);

            //
            // Rounds 64-79
            //
            // left
            b = RL(b + F5(c, d, e) + _x[4] + unchecked((int)0xa953fd4e), 9) + a;
            d = RL(d, 10);
            a = RL(a + F5(b, c, d) + _x[0] + unchecked((int)0xa953fd4e), 15) + e;
            c = RL(c, 10);
            e = RL(e + F5(a, b, c) + _x[5] + unchecked((int)0xa953fd4e), 5) + d;
            b = RL(b, 10);
            d = RL(d + F5(e, a, b) + _x[9] + unchecked((int)0xa953fd4e), 11) + c;
            a = RL(a, 10);
            c = RL(c + F5(d, e, a) + _x[7] + unchecked((int)0xa953fd4e), 6) + b;
            e = RL(e, 10);
            b = RL(b + F5(c, d, e) + _x[12] + unchecked((int)0xa953fd4e), 8) + a;
            d = RL(d, 10);
            a = RL(a + F5(b, c, d) + _x[2] + unchecked((int)0xa953fd4e), 13) + e;
            c = RL(c, 10);
            e = RL(e + F5(a, b, c) + _x[10] + unchecked((int)0xa953fd4e), 12) + d;
            b = RL(b, 10);
            d = RL(d + F5(e, a, b) + _x[14] + unchecked((int)0xa953fd4e), 5) + c;
            a = RL(a, 10);
            c = RL(c + F5(d, e, a) + _x[1] + unchecked((int)0xa953fd4e), 12) + b;
            e = RL(e, 10);
            b = RL(b + F5(c, d, e) + _x[3] + unchecked((int)0xa953fd4e), 13) + a;
            d = RL(d, 10);
            a = RL(a + F5(b, c, d) + _x[8] + unchecked((int)0xa953fd4e), 14) + e;
            c = RL(c, 10);
            e = RL(e + F5(a, b, c) + _x[11] + unchecked((int)0xa953fd4e), 11) + d;
            b = RL(b, 10);
            d = RL(d + F5(e, a, b) + _x[6] + unchecked((int)0xa953fd4e), 8) + c;
            a = RL(a, 10);
            c = RL(c + F5(d, e, a) + _x[15] + unchecked((int)0xa953fd4e), 5) + b;
            e = RL(e, 10);
            b = RL(b + F5(c, d, e) + _x[13] + unchecked((int)0xa953fd4e), 6) + a;
            d = RL(d, 10);

            // right
            bb = RL(bb + F1(cc, dd, ee) + _x[12], 8) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F1(bb, cc, dd) + _x[15], 5) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F1(aa, bb, cc) + _x[10], 12) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F1(ee, aa, bb) + _x[4], 9) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F1(dd, ee, aa) + _x[1], 12) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F1(cc, dd, ee) + _x[5], 5) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F1(bb, cc, dd) + _x[8], 14) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F1(aa, bb, cc) + _x[7], 6) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F1(ee, aa, bb) + _x[6], 8) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F1(dd, ee, aa) + _x[2], 13) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F1(cc, dd, ee) + _x[13], 6) + aa;
            dd = RL(dd, 10);
            aa = RL(aa + F1(bb, cc, dd) + _x[14], 5) + ee;
            cc = RL(cc, 10);
            ee = RL(ee + F1(aa, bb, cc) + _x[0], 15) + dd;
            bb = RL(bb, 10);
            dd = RL(dd + F1(ee, aa, bb) + _x[3], 13) + cc;
            aa = RL(aa, 10);
            cc = RL(cc + F1(dd, ee, aa) + _x[9], 11) + bb;
            ee = RL(ee, 10);
            bb = RL(bb + F1(cc, dd, ee) + _x[11], 11) + aa;
            dd = RL(dd, 10);

            dd += c + _h1;
            _h1 = _h2 + d + ee;
            _h2 = _h3 + e + aa;
            _h3 = _h4 + a + bb;
            _h4 = _h0 + b + cc;
            _h0 = dd;

            //
            // reset the offset and clean out the word buffer.
            //
            _xOff = 0;
            for (int i = 0; i != _x.Length; i++)
            {
                _x[i] = 0;
            }
        }

        // public override IMemoable Copy() { return new RipeMD160Digest(this); }

        // public override void Reset(IMemoable other) { RipeMD160Digest d = (RipeMD160Digest)other; CopyIn(d); }

    }
}
