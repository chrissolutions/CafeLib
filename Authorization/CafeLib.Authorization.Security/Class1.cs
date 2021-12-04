using System;
using System.Collections.Generic;
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
    }
}
