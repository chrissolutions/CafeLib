using CafeLib.Bitcoin.Numerics;
using CafeLib.Bitcoin.Scripting;

namespace CafeLib.Bitcoin.Chain
{
    public class Signature
    {
        private UInt256 _r;
        private UInt256 _s;
        private SignatureHashType _hashType;

        public Signature()
        {
            _r = UInt256.One;
            _s = UInt256.One;
            _hashType = new SignatureHashType(SignatureHashEnum.All);
        }

        public Signature(UInt256 r, UInt256 s, SignatureHashType hashType)
        {
            _r = r;
            _s = s;
            _hashType = hashType;
        }

        public Signature(VarType signature)
            : this()
        {
            if (!signature.IsEmpty)
            {
                var hashType = new SignatureHashType(signature.LastByte);
                var derBuffer = signature.Slice(0, (int)signature.Length - 1);
                //this.fromDer(derbuf, false)
                _hashType = hashType;
            }
        }

        public static bool IsTxDer(VarType vchSig)
        {
            return true;
        }

        /// <summary>
        /// Compares to bitcoind's IsLowDERSignature
        /// See also Ecdsa signature algorithm which enforces this.
        /// See also Bip 62, "low S values in signatures"
        /// </summary>
        /// <returns></returns>
        public bool HasLowS()
        {
            //if (
            //    this.s.lt(1) ||
            //    this.s.gt(
            //        Bn.fromBuffer(
            //            Buffer.from(
            //                '7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5D576E7357A4501DDFE92F46681B20A0',
            //                'hex'
            //            )
            //        )
            //    )
            //)
            //{
            //    return false
            //}
            return true;
        }


        public static Signature FromTxFormat(VarType vchSig)
        {
            if (vchSig.IsEmpty)
            {
                return new Signature();
            }

            var hashType = vchSig.LastByte;
            //const derbuf = buf.slice(0, buf.length - 1)
            //this.fromDer(derbuf, false)
            //this.nHashType = nHashType
            return new Signature();
        }
    }
}
