using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace CafeLib.Core.Security
{
	public class PasswordHashOptions : IOptions<PasswordHashOptions>
	{
        private PasswordHashAlgorithm _hashAlgorithm;

		/// <summary>
		/// 160-bit hash by default for SHA1
		/// 256-bit hash by default for SHA256
		/// 384-bit hash by default for SHA384
		/// 512-bit hash by default for SHA512
		/// </summary>
		public int HashSize { get; private set; }

		/// <summary>
		/// 64-bit salt is minimally acceptable
		/// </summary>
		public int SaltSize { get; set; }

		/// <summary>
		/// One iteration is minimally acceptable
		/// </summary>
		public int Iterations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEqualityComparer<byte[]> ByteArrayComparer { get; set; } = new BytesEqualityComparer();
        
        /// <summary>
        /// 
        /// </summary>
        public PasswordHashOptions Value => this;


        /// <summary>
        /// Creates a default instance with implementation of the SHA256 algorithm
        /// </summary>
        public PasswordHashOptions() 
            : this(PasswordHashAlgorithm.Sha256)
		{
		}

		/// <summary>
		/// Creates an instance with specified algorithm and parameters
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="saltSize"></param>
		/// <param name="iterations"></param>
		public PasswordHashOptions(PasswordHashAlgorithm algorithm, int? saltSize = null, int? iterations = null)
		{
			HashAlgorithm = algorithm;

            switch (HashAlgorithm)
            {
                case PasswordHashAlgorithm.Sha1:
                    SaltSize = saltSize ?? 10; // hashed password will contain 40 characters for saltSize = 10
                    Iterations = iterations ?? 1024;
                    break;

                case PasswordHashAlgorithm.Sha256:
                    SaltSize = saltSize ?? 16; // hashed password will contain 64 characters for saltSize = 16
                    Iterations = iterations ?? 8192;
                    break;

                case PasswordHashAlgorithm.Sha384:
                    SaltSize = saltSize ?? 24; // hashed password will contain 96 characters for saltSize = 24
                    Iterations = iterations ?? 10240;
                    break;

                case PasswordHashAlgorithm.Sha512:
                    SaltSize = saltSize ?? 32; // hashed password will contain 128 characters for saltSize = 32
                    Iterations = iterations ?? 10240;
                    break;
		    }
    	}

        /// <summary>
        /// algorithm for hashing
        /// </summary>
        public PasswordHashAlgorithm HashAlgorithm
        {
            get => _hashAlgorithm;
            set
            {
                if (_hashAlgorithm == value) return;

                _hashAlgorithm = value;
                switch (_hashAlgorithm)
                {
                    case PasswordHashAlgorithm.Sha1:
                        HashSize = 20;
                        break;

                    case PasswordHashAlgorithm.Sha256:
                        HashSize = 32;
                        break;

                    case PasswordHashAlgorithm.Sha384:
                        HashSize = 48;
                        break;

                    case PasswordHashAlgorithm.Sha512:
                        HashSize = 64;
                        break;
                }
            }
        }
    }
}
