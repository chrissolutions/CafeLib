// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable
using System.Security.Cryptography;

namespace CafeLib.Core.Security
{
    public sealed class AesCryptoServiceProvider : Aes
    {
        private readonly Aes _impl;

        public AesCryptoServiceProvider()
        {
            // This class wraps Aes
            _impl = Create();
            _impl.FeedbackSize = 8;
        }

        public override int FeedbackSize
        {
            get => _impl.FeedbackSize;
            set => _impl.FeedbackSize = value;
        }

        public override int BlockSize
        {
            get => _impl.BlockSize;
            set => _impl.BlockSize = value;
        }

        public override byte[] IV
        {
            get => _impl.IV;
            set => _impl.IV = value;
        }

        public override byte[] Key
        {
            get => _impl.Key;
            set => _impl.Key = value;
        }

        public override int KeySize
        {
            get => _impl.KeySize;
            set => _impl.KeySize = value;
        }

        public override CipherMode Mode
        {
            get => _impl.Mode;
            set => _impl.Mode = value;
        }

        public override PaddingMode Padding
        {
            get => _impl.Padding;
            set => _impl.Padding = value;
        }

        public override KeySizes[] LegalBlockSizes => _impl.LegalBlockSizes;
        public override KeySizes[] LegalKeySizes => _impl.LegalKeySizes;

        public override ICryptoTransform CreateEncryptor()
            => _impl.CreateEncryptor();
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIv)
            => _impl.CreateEncryptor(rgbKey, rgbIv);
        public override ICryptoTransform CreateDecryptor()
            => _impl.CreateDecryptor();
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIv) 
            => _impl.CreateDecryptor(rgbKey, rgbIv);
        public override void GenerateIV() 
            => _impl.GenerateIV();
        public override void GenerateKey() 
            => _impl.GenerateKey();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _impl.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
