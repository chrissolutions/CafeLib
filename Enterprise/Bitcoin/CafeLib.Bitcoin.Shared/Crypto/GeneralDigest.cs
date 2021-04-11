using System;

namespace CafeLib.Bitcoin.Shared.Crypto
{
    internal abstract class GeneralDigest
    {
        private const int ByteLength = 64;

        private readonly byte[] _xBuf;
        private int _xBufOff;
        private long _byteCount;

        internal GeneralDigest()
        {
            _xBuf = new byte[4];
        }

        internal GeneralDigest(GeneralDigest t)
        {
            _xBuf = new byte[t._xBuf.Length];
            CopyIn(t);
        }

        protected void CopyIn(GeneralDigest t)
        {
            Array.Copy(t._xBuf, 0, _xBuf, 0, t._xBuf.Length);

            _xBufOff = t._xBufOff;
            _byteCount = t._byteCount;
        }

        public void Update(byte input)
        {
            _xBuf[_xBufOff++] = input;

            if (_xBufOff == _xBuf.Length)
            {
                ProcessWord(_xBuf, 0);
                _xBufOff = 0;
            }

            _byteCount++;
        }

        public void BlockUpdate(ReadOnlySpan<byte> input)
        {
            var length = input.Length;
            //
            // fill the current word
            //
            int i = 0;
            if (_xBufOff != 0)
            {
                while (i < length)
                {
                    _xBuf[_xBufOff++] = input[i++];
                    if (_xBufOff == 4)
                    {
                        ProcessWord(_xBuf, 0);
                        _xBufOff = 0;
                        break;
                    }
                }
            }

            //
            // process whole words.
            //
            int limit = ((length - i) & ~3) + i;
            for (; i < limit; i += 4)
            {
                ProcessWord(input, i);
            }

            //
            // load in the remainder.
            //
            while (i < length)
            {
                _xBuf[_xBufOff++] = input[i++];
            }

            _byteCount += length;
        }

        public void Finish()
        {
            long bitLength = _byteCount << 3;

            //
            // add the pad bytes.
            //
            Update((byte)128);

            while (_xBufOff != 0)
                Update((byte)0);
            ProcessLength(bitLength);
            ProcessBlock();
        }

        public virtual void Reset()
        {
            _byteCount = 0;
            _xBufOff = 0;
            Array.Clear(_xBuf, 0, _xBuf.Length);
        }

        public int GetByteLength()
        {
            return ByteLength;
        }

        internal abstract void ProcessWord(ReadOnlySpan<byte> input, int inOff);
        internal abstract void ProcessLength(long bitLength);
        internal abstract void ProcessBlock();
        public abstract string AlgorithmName
        {
            get;
        }
        public abstract int GetDigestSize();
        public abstract int DoFinal(Span<byte> output);
        //public abstract IMemoable Copy();
        //public abstract void Reset(IMemoable t);
    }
}
