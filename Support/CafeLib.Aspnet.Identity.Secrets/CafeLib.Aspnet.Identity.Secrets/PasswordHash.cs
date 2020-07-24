using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public class PasswordHash : IPasswordHash
	{
		private readonly PasswordHashOptions _options;
		private readonly IEqualityComparer<byte[]> _comparer;

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
		}

		/// <summary>
		/// Hash password.
		/// </summary>
		/// <param name="password"></param>
		/// <returns>hash of the password</returns>
		public string HashPassword(string password)
		{
			byte[] saltBuffer;
			byte[] hashBuffer;

			using (var keyDerivation = new Rfc2898DeriveBytes(password, _options.SaltSize, _options.Iterations, _options.HashAlgorithmName))
			{
				saltBuffer = keyDerivation.Salt;
				hashBuffer = keyDerivation.GetBytes(_options.HashSize);
			}

			var result = new byte[_options.HashSize + _options.SaltSize];
			Buffer.BlockCopy(hashBuffer, 0, result, 0, _options.HashSize);
			Buffer.BlockCopy(saltBuffer, 0, result, _options.HashSize, _options.SaltSize);
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

			var hashBytes = new byte[_options.HashSize];
			Buffer.BlockCopy(hashedPasswordBytes, 0, hashBytes, 0, _options.HashSize);
			var saltBytes = new byte[_options.SaltSize];
			Buffer.BlockCopy(hashedPasswordBytes, _options.HashSize, saltBytes, 0, _options.SaltSize);

            using var keyDerivation = new Rfc2898DeriveBytes(providedPassword, saltBytes, _options.Iterations, _options.HashAlgorithmName);
            var providedHashBytes = keyDerivation.GetBytes(_options.HashSize);

            return _comparer.Equals(hashBytes, providedHashBytes);
		}
	}
}
