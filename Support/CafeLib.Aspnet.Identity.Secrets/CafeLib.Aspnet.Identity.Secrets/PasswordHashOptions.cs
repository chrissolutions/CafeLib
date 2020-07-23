using System.Security.Cryptography;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public class PasswordHashOptions
	{
        private PasswordHashAlgorithms _hashAlgorithm;

		/// <summary>
		/// The name of a cryptographic hash algorithm
		/// </summary>
		public HashAlgorithmName HashAlgorithmName { get; private set; }

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
		/// 1 iteration is minimally acceptable
		/// </summary>
		public int Iterations { get; set; }

		/// <summary>
		/// Creates a default instance with implementation of the SHA256 algorithm
		/// </summary>
		public PasswordHashOptions() : this(PasswordHashAlgorithms.Sha256)
		{
		}

		/// <summary>
		/// Creates an instance with specified algorithm and parameters
		/// </summary>
		/// <param name="algorithm"></param>
		/// <param name="saltSize"></param>
		/// <param name="iterations"></param>
		public PasswordHashOptions(PasswordHashAlgorithms algorithm, int? saltSize = null, int? iterations = null)
		{
			HashAlgorithm = algorithm;

            switch (HashAlgorithm)
            {
                case PasswordHashAlgorithms.Sha1:
                    SaltSize = saltSize ?? 10; // hashed password will contain 40 characters for saltSize = 10
                    Iterations = iterations ?? 1024;
                    break;

                case PasswordHashAlgorithms.Sha256:
                    SaltSize = saltSize ?? 16; // hashed password will contain 64 characters for saltSize = 16
                    Iterations = iterations ?? 8192;
                    break;

                case PasswordHashAlgorithms.Sha384:
                    SaltSize = saltSize ?? 24; // hashed password will contain 96 characters for saltSize = 24
                    Iterations = iterations ?? 10240;
                    break;

                case PasswordHashAlgorithms.Sha512:
                    SaltSize = saltSize ?? 32; // hashed password will contain 128 characters for saltSize = 32
                    Iterations = iterations ?? 10240;
                    break;
		    }
    	}

        /// <summary>
        /// algorithm for hashing
        /// </summary>
        public PasswordHashAlgorithms HashAlgorithm
        {
            get => _hashAlgorithm;
            set
            {
                if (_hashAlgorithm == value) return;

                _hashAlgorithm = value;
                switch (_hashAlgorithm)
                {
                    case PasswordHashAlgorithms.Sha1:
                        HashAlgorithmName = HashAlgorithmName.SHA1;
                        HashSize = 20;
                        break;

                    case PasswordHashAlgorithms.Sha256:
                        HashAlgorithmName = HashAlgorithmName.SHA256;
                        HashSize = 32;
                        break;

                    case PasswordHashAlgorithms.Sha384:
                        HashAlgorithmName = HashAlgorithmName.SHA384;
                        HashSize = 48;
                        break;

                    case PasswordHashAlgorithms.Sha512:
                        HashAlgorithmName = HashAlgorithmName.SHA512;
                        HashSize = 64;
                        break;
                }
            }
        }
	}
}
