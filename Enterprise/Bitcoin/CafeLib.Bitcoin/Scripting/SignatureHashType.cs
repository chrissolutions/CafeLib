using System;
using CafeLib.Bitcoin.Numerics;

namespace CafeLib.Bitcoin.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public class SignatureHashType
    {
        private SignatureHashEnum SignatureHash => (SignatureHashEnum)RawSigHashType;

        public SignatureHashType()
        {
            RawSigHashType = (uint)SignatureHashEnum.All;
        }

        public SignatureHashType(SignatureHashEnum sigHash)
        {
            RawSigHashType = (uint)sigHash;
        }

        public SignatureHashType(uint sigHash)
        {
            RawSigHashType = sigHash;
        }

        public SignatureHashType(VarType vchSig)
        {
            RawSigHashType = vchSig.Length == 0
                ? (uint) SignatureHashEnum.Unsupported
                : (uint) (SignatureHashEnum) vchSig.LastByte;
        }

        public bool IsDefined 
        {
            get
            {
                var baseType = SignatureHash & ~(SignatureHashEnum.ForkId | SignatureHashEnum.AnyoneCanPay);
                return baseType >= SignatureHashEnum.All && baseType <= SignatureHashEnum.Single;
            }
        }

        public bool HasForkId => (SignatureHash & SignatureHashEnum.ForkId) != 0;

        public bool HasAnyoneCanPay => (SignatureHash & SignatureHashEnum.AnyoneCanPay) != 0;

        public UInt32 RawSigHashType { get; }

        public SignatureHashType WithBaseType(BaseSignatureHashEnum baseSigHashType)
        {
            return new SignatureHashType((RawSigHashType & ~(uint)0x1f) | (uint)baseSigHashType);
        }

        public SignatureHashType WithForkValue(uint forkId)
        {
            return new SignatureHashType((forkId << 8) | (RawSigHashType & 0xff));
        }

        public SignatureHashType WithForkId(bool forkId = true)
        {
            return new SignatureHashType((RawSigHashType & ~(uint)SignatureHashEnum.ForkId) | (forkId ? (uint)SignatureHashEnum.ForkId : 0));
        }

        public SignatureHashType WithAnyoneCanPay(bool anyoneCanPay = true)
        {
            return new SignatureHashType((RawSigHashType & ~(uint)SignatureHashEnum.AnyoneCanPay) | (anyoneCanPay ? (uint)SignatureHashEnum.AnyoneCanPay : 0));
        }

        public BaseSignatureHashEnum GetBaseType() { return (BaseSignatureHashEnum)((int)RawSigHashType & 0x1f); }

        public bool IsBaseNone => ((int)RawSigHashType & 0x1f) == (int)BaseSignatureHashEnum.None;

        public bool IsBaseSingle => ((int)RawSigHashType & 0x1f) == (int)BaseSignatureHashEnum.Single;

        public uint GetForkValue() { return RawSigHashType >> 8; }

        public override string ToString() {
            return string.Empty;
        }
    }
}