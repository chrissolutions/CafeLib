using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public class PasswordHasher : IPasswordHasher
	{
		private readonly PasswordHasherOptions _options;

		private readonly IEqualityComparer<byte[]> _comparer;

		/// <summary>
		/// Creates a new instance of <see cref="PasswordHasher"/>.
		/// </summary>
		/// <param name="optionsAccessor">The options for this instance.</param>
		/// <param name="bytesComparer">The comparer of byte[] for this instance.</param>
		public PasswordHasher(IOptions<PasswordHasherOptions> optionsAccessor = null, IEqualityComparer<byte[]> bytesComparer = null)
		{
			_options = optionsAccessor?.Value ?? new PasswordHasherOptions();

			if (_options.SaltSize < 8)
				throw new ArgumentOutOfRangeException(nameof(_options.SaltSize));

			if (_options.Iterations < 1)
				throw new ArgumentOutOfRangeException(nameof(_options.Iterations));

			_comparer = bytesComparer ?? new BytesEqualityComparer();
		}

		public string HashPassword(string password)
		{
			byte[] saltBuffer;
			byte[] hashBuffer;

			using (var keyDerivation = new Rfc2898DeriveBytes(password, _options.SaltSize, _options.Iterations, _options.HashAlgorithmName))
			{
				saltBuffer = keyDerivation.Salt;
				hashBuffer = keyDerivation.GetBytes(_options.HashSize);
			}

			byte[] result = new byte[_options.HashSize + _options.SaltSize];
			Buffer.BlockCopy(hashBuffer, 0, result, 0, _options.HashSize);
			Buffer.BlockCopy(saltBuffer, 0, result, _options.HashSize, _options.SaltSize);
			return Convert.ToBase64String(result);
		}

		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);
			if (hashedPasswordBytes.Length != _options.HashSize + _options.SaltSize)
			{
				return false;
			}

			byte[] hashBytes = new byte[_options.HashSize];
			Buffer.BlockCopy(hashedPasswordBytes, 0, hashBytes, 0, _options.HashSize);
			byte[] saltBytes = new byte[_options.SaltSize];
			Buffer.BlockCopy(hashedPasswordBytes, _options.HashSize, saltBytes, 0, _options.SaltSize);

            using var keyDerivation = new Rfc2898DeriveBytes(providedPassword, saltBytes, _options.Iterations, _options.HashAlgorithmName);
            var providedHashBytes = keyDerivation.GetBytes(_options.HashSize);

            return _comparer.Equals(hashBytes, providedHashBytes);
		}
		
	}
}
