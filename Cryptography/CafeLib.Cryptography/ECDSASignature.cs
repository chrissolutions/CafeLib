using System;
using System.Collections.Generic;
using System.IO;
using CafeLib.Cryptography.BouncyCastle.Asn1;
using CafeLib.Cryptography.BouncyCastle.Math;

namespace CafeLib.Cryptography
{
	public class ECDSASignature
	{
        public BigInteger R { get; }

        public BigInteger S { get; }

        public ECDSASignature(BigInteger r, BigInteger s)
		{
			R = r;
			S = s;
		}

		public ECDSASignature(IReadOnlyList<BigInteger> rs)
		{
			R = rs[0];
			S = rs[1];
		}

		/**
		* What we get back from the signer are the two components of a signature, r and s. To get a flat byte stream
		* of the type used by Bitcoin we have to encode them using DER encoding, which is just a way to pack the two
		* components into a structure.
		*/
		public byte[] ToDER()
		{
			// Usually 70-72 bytes.
			var bos = new MemoryStream(72);
			var seq = new DerSequenceGenerator(bos);
			seq.AddObject(new DerInteger(R));
			seq.AddObject(new DerInteger(S));
			seq.Close();
			return bos.ToArray();

		}
		const string InvalidDERSignature = "Invalid DER signature";
		public static ECDSASignature FromDER(byte[] sig)
		{
			try
			{
				Asn1InputStream decoder = new Asn1InputStream(sig);
				var seq = decoder.ReadObject() as DerSequence;
				if (seq == null || seq.Count != 2)
					throw new FormatException(InvalidDERSignature);
				return new ECDSASignature(((DerInteger)seq[0]).Value, ((DerInteger)seq[1]).Value);
			}
			catch (IOException ex)
			{
				throw new FormatException(InvalidDERSignature, ex);
			}
		}

		public ECDSASignature MakeCanonical()
        {
            return S.CompareTo(ECKey.HALF_CURVE_ORDER) > 0
                ? new ECDSASignature(R, ECKey.CreateCurve().N.Subtract(S))
                : this;
        }

		public static bool IsValidDER(byte[] bytes)
		{
			try
			{
				FromDER(bytes);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
