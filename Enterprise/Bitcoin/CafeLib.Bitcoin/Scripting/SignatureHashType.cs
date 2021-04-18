using System;

namespace CafeLib.Bitcoin.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    public class SignatureHashType
    {
        private readonly uint _signatureHash;

        private SignatureHashEnum SignatureHash => (SignatureHashEnum)_signatureHash;

        public SignatureHashType()
        {
            _signatureHash = (uint)SignatureHashEnum.All;
        }

        public SignatureHashType(SignatureHashEnum sigHash)
        {
            _signatureHash = (uint)sigHash;
        }

        public SignatureHashType(uint sigHash)
        {
            _signatureHash = sigHash;
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

        public UInt32 rawSigHashType => _signatureHash;

        public SignatureHashType WithBaseType(BaseSignatureHashEnum baseSigHashType)
        {
            return new SignatureHashType((_signatureHash & ~(uint)0x1f) | (uint)baseSigHashType);
        }

        public SignatureHashType WithForkValue(uint forkId)
        {
            return new SignatureHashType((forkId << 8) | (_signatureHash & 0xff));
        }

        public SignatureHashType WithForkId(bool forkId = true)
        {
            return new SignatureHashType((_signatureHash & ~(uint)SignatureHashEnum.ForkId) | (forkId ? (uint)SignatureHashEnum.ForkId : 0));
        }

        public SignatureHashType WithAnyoneCanPay(bool anyoneCanPay = true)
        {
            return new SignatureHashType((_signatureHash & ~(uint)SignatureHashEnum.AnyoneCanPay) | (anyoneCanPay ? (uint)SignatureHashEnum.AnyoneCanPay : 0));
        }

        public BaseSignatureHashEnum GetBaseType() { return (BaseSignatureHashEnum)((int)_signatureHash & 0x1f); }

        public bool IsBaseNone => ((int)_signatureHash & 0x1f) == (int)BaseSignatureHashEnum.None;

        public bool IsBaseSingle => ((int)_signatureHash & 0x1f) == (int)BaseSignatureHashEnum.Single;

        public uint GetForkValue() { return _signatureHash >> 8; }

        public override string ToString() {
            return string.Empty;
        }
    }
}