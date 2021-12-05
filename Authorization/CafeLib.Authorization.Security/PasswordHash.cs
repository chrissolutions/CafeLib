using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Cryptography.BouncyCastle.Crypto;
using CafeLib.Cryptography.BouncyCastle.Crypto.Digests;
using CafeLib.Cryptography.BouncyCastle.Crypto.Generators;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;
using CafeLib.Cryptography.BouncyCastle.Security;
using Microsoft.Extensions.Options;

namespace CafeLib.Authorization.Security
{
	public class PasswordHash : IPasswordHash
	{
		private readonly PasswordHashOptions _options;
		private readonly IEqualityComparer<byte[]> _comparer;
        private readonly IDigest _digest;

		/// <summary>
		/// Default static password hash;
		/// </summary>
		public static readonly PasswordHash Default = new();

		/// <summary>
		/// PasswordHash constructor.
		/// </summary>
		/// <param name="options">The options for this instance.</param>
		public PasswordHash(IOptions<PasswordHashOptions> options = null)
		{
			_options = options?.Value ?? new PasswordHashOptions();

			if (_options.SaltSize < 8)
				throw new ArgumentOutOfRangeException(nameof(_options.SaltSize));

			if (_options.Iterations < 1)
				throw new ArgumentOutOfRangeException(nameof(_options.Iterations));

			_comparer = _options.ByteArrayComparer ?? new BytesEqualityComparer();

            _digest = _options.HashAlgorithm switch
            {
                PasswordHashAlgorithm.Md5 => new MD5Digest(),
                PasswordHashAlgorithm.Sha1 => new Sha1Digest(),
                PasswordHashAlgorithm.Sha256 => new Sha256Digest(),
                PasswordHashAlgorithm.Sha384 => new Sha384Digest(),
                PasswordHashAlgorithm.Sha512 => new Sha512Digest(),
                _ => new Sha256Digest()
            };
		}

		/// <summary>
		/// Hash password.
		/// </summary>
		/// <param name="password"></param>
		/// <returns>hash of the password</returns>
		public string HashPassword(string password)
		{
            var random = new SecureRandom();
			var salt = random.GenerateSeed(_options.SaltSize);

            var generator = new Pkcs5S2ParametersGenerator(_digest);
            generator.Init(Encoding.UTF8.GetBytes(password), salt, _options.Iterations);

            var hash = ((KeyParameter)generator.GenerateDerivedMacParameters(8 * _options.HashSize)).GetKey();

			var result = new byte[_options.HashSize + _options.SaltSize];
			Buffer.BlockCopy(hash, 0, result, 0, _options.HashSize);
			Buffer.BlockCopy(salt, 0, result, _options.HashSize, _options.SaltSize);
			return Convert.ToBase64String(result);
		}

		/// <summary>
		/// Verify hashed password.
		/// </summary>
		/// <param name="hashedPassword"></param>
		/// <param name="providedPassword"></param>
		/// <returns></returns>
		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
			if (hashedPasswordBytes.Length != _options.HashSize + _options.SaltSize)
			{
				return false;
			}

			var hash = new byte[_options.HashSize];
			Buffer.BlockCopy(hashedPasswordBytes, 0, hash, 0, _options.HashSize);
			var salt = new byte[_options.SaltSize];
			Buffer.BlockCopy(hashedPasswordBytes, _options.HashSize, salt, 0, _options.SaltSize);

            var generator = new Pkcs5S2ParametersGenerator(_digest);
            generator.Init(Encoding.UTF8.GetBytes(providedPassword), salt, _options.Iterations);
            var providedHashBytes = ((KeyParameter)generator.GenerateDerivedMacParameters(8 * _options.HashSize)).GetKey();

			return _comparer.Equals(hash, providedHashBytes);
		}
	}
}
