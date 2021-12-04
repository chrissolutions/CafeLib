using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using CafeLib.Cryptography.BouncyCastle.Crypto.Generators;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;
using CafeLib.Cryptography.BouncyCastle.Security;

namespace CafeLib.Authorization.Security
{
    internal class Class1
    {
        public void HashPassword()
        {
            const string passwordToSave = "password 1";

            // tuning parameters

            // these sizes are relatively arbitrary
            int seedBytes = 20;
            int hashBytes = 20;

            // increase iterations as high as your performance can tolerate
            // since this increases computational cost of password guessing
            // which should help security
            int iterations = 1000;

            // to save a new password:

            var rng = new SecureRandom();
            byte[] salt = rng.GenerateSeed(seedBytes);

            Pkcs5S2ParametersGenerator kdf = new Pkcs5S2ParametersGenerator();
            kdf.Init(Encoding.UTF8.GetBytes(passwordToSave), salt, iterations);

            byte[] hash =
                ((KeyParameter)kdf.GenerateDerivedMacParameters(8 * hashBytes)).GetKey();

            // now save salt and hash

            // to check a password, given the known previous salt and hash:

            kdf = new Pkcs5S2ParametersGenerator();
            //kdf.Init(passwordToCheck.getBytes("UTF-8"), salt, iterations);

            byte[] hashToCheck =
                ((KeyParameter)kdf.GenerateDerivedMacParameters(8 * hashBytes)).GetKey();

            // if the bytes of hashToCheck don't match the bytes of hash
            // that means the password is invalid
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

            using (var keyDerivation = new Rfc2898DeriveBytes(password, _options.SaltSize, _options.Iterations, _hashName))
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

            using var keyDerivation = new Rfc2898DeriveBytes(providedPassword, saltBytes, _options.Iterations, _hashName);
            var providedHashBytes = keyDerivation.GetBytes(_options.HashSize);

            return _comparer.Equals(hashBytes, providedHashBytes);
        }

	}
}
