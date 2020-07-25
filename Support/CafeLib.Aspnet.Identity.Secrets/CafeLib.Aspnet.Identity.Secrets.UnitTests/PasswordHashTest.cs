using System;
using Microsoft.Extensions.Options;
using Xunit;

namespace CafeLib.Aspnet.Identity.Secrets.UnitTests
{
	public class PasswordHashTest
	{
		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		[InlineData(7)]
		public void Ctor_InvalidSaltSize_Throws(int saltSize)
		{
			// Act & assert
			Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new PasswordHash(BuildOptions(saltSize: saltSize));
            });
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Ctor_InvalidIterations_Throws(int iterations)
		{
			// Act & assert
			Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new PasswordHash(BuildOptions(iterations: iterations));
            });
		}

		[Theory]
		[InlineData(PasswordHashAlgorithm.Sha1)]
		[InlineData(PasswordHashAlgorithm.Sha256)]
		[InlineData(PasswordHashAlgorithm.Sha384)]
		[InlineData(PasswordHashAlgorithm.Sha512)]
		public void HashRoundTrip(PasswordHashAlgorithm algorithm)
		{
			// Arrange
			var hasher = new PasswordHash(BuildOptions(algorithm));

			// Act & assert - success case
			var hashedPassword = hasher.HashPassword("password 1");
			var successResult = hasher.VerifyHashedPassword(hashedPassword, "password 1");
			Assert.True(successResult);

			// Act & assert - failure case
			var failedResult = hasher.VerifyHashedPassword(hashedPassword, "password 2");
			Assert.False(failedResult);
		}

		[Theory]
		[InlineData(PasswordHashAlgorithm.Sha1, 8, 40)]
		[InlineData(PasswordHashAlgorithm.Sha1, 9, 40)]
		[InlineData(PasswordHashAlgorithm.Sha1, 10, 40)]
		[InlineData(PasswordHashAlgorithm.Sha1, 16, 48)]
		[InlineData(PasswordHashAlgorithm.Sha256, 16, 64)]
		[InlineData(PasswordHashAlgorithm.Sha384, 24, 96)]
		[InlineData(PasswordHashAlgorithm.Sha512, 32, 128)]
		public void LengthRoundTrip(PasswordHashAlgorithm algorithm, int saltSize, int expectedLength)
		{
			// Arrange
			var hasher = new PasswordHash(BuildOptions(algorithm, saltSize));

			// Act & assert - success case
			var hashedPassword = hasher.HashPassword("password 1");
			Assert.Equal(expectedLength, hashedPassword.Length);
		}

		[Theory]
		[InlineData(PasswordHashAlgorithm.Sha1, 20)]
		[InlineData(PasswordHashAlgorithm.Sha256, 32)]
		[InlineData(PasswordHashAlgorithm.Sha384, 48)]
		[InlineData(PasswordHashAlgorithm.Sha512, 64)]
		public void OptionsRoundTrip(PasswordHashAlgorithm algorithm, int expectedSize)
		{
			// Arrange
			var options = new PasswordHashOptions(algorithm);

			// Act & assert - success case
			Assert.Equal(expectedSize, options.HashSize);

			// Arrange
			options.HashAlgorithm = PasswordHashAlgorithm.Sha1;
			options.SaltSize = expectedSize;
			options.HashAlgorithm = algorithm;

			// Act & assert - failure case
			Assert.Equal(expectedSize, options.HashSize);
		}

		public static IOptions<PasswordHashOptions> BuildOptions(int? saltSize = null, int? iterations = null)
		{
			var options = new PasswordHashOptions();

			if (saltSize.HasValue)
				options.SaltSize = saltSize.Value;

			if (iterations.HasValue)
				options.Iterations = iterations.Value;

			return Options.Create(options);
		}

		public static IOptions<PasswordHashOptions> BuildOptions(PasswordHashAlgorithm algorithm, int? saltSize = null, int? iterations = null)
		{
			var options = new PasswordHashOptions(algorithm, saltSize, iterations);
			return Options.Create(options);
		}
	}
}
