using System;
using System.IO;
using System.Text;
using CafeLib.Core.Encodings;
using CafeLib.Core.Numerics;
using CafeLib.Cryptography.BouncyCastle.Asn1;
using CafeLib.Cryptography.BouncyCastle.Asn1.Sec;
using CafeLib.Cryptography.BouncyCastle.Asn1.X9;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;
using CafeLib.Cryptography.BouncyCastle.Crypto.Signers;
using CafeLib.Cryptography.BouncyCastle.Math;
using CafeLib.Cryptography.BouncyCastle.Math.EC;

namespace CafeLib.Cryptography
{
	public class ECKey
    {
        private static readonly HexEncoder HexEncoder = new HexEncoder();
        private readonly ECKeyParameters _key;

        public static BigInteger HalfCurveOrder;
        public static ECDomainParameters Curve;

		public ECPrivateKeyParameters PrivateKey => _key as ECPrivateKeyParameters;

		static ECKey()
		{
			var parameters = CreateCurve();
			Curve = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
			HalfCurveOrder = parameters.N.ShiftRight(1);
		}

		public ECKey(byte[] vch, bool isPrivate)
		{
			if (isPrivate)
				_key = new ECPrivateKeyParameters(new BigInteger(1, vch), DomainParameter);
			else
			{
				var q = Secp256k1.Curve.DecodePoint(vch);
				_key = new ECPublicKeyParameters("EC", q, DomainParameter);
			}
		}

		X9ECParameters _secp256k1;
		public X9ECParameters Secp256k1 => _secp256k1 ??= CreateCurve();

        public static X9ECParameters CreateCurve()
		{
			return SecNamedCurves.GetByName("secp256k1");
		}

        private ECDomainParameters _domainParameter;
		public ECDomainParameters DomainParameter => _domainParameter ??= new ECDomainParameters(Secp256k1.Curve, Secp256k1.G, Secp256k1.N, Secp256k1.H);


        public ECDSASignature Sign(UInt256 hash)
		{
			AssertPrivateKey();
			var signer = new DeterministicECDSA();
			signer.SetPrivateKey(PrivateKey);
			var sig = ECDSASignature.FromDER(signer.SignHash(hash));
			return sig.MakeCanonical();
		}

		private void AssertPrivateKey()
		{
			if (PrivateKey == null)
				throw new InvalidOperationException("This key should be a private key for such operation");
		}

		internal bool Verify(UInt256 hash, ECDSASignature sig)
		{
			var signer = new ECDsaSigner();
			signer.Init(false, GetPublicKeyParameters());
			return signer.VerifySignature(hash, sig.R, sig.S);
		}

		public byte[] GetPublicKeyPoint(bool isCompressed)
		{
			var q = GetPublicKeyParameters().Q;
			//Pub key (q) is composed into X and Y, the compressed form only include X, which can derive Y along with 02 or 03 prepent depending on whether Y in even or odd.
			var result = Secp256k1.Curve.CreatePoint(q.X.ToBigInteger(), q.Y.ToBigInteger(), isCompressed).GetEncoded();
			return result;
		}

		public ECPublicKeyParameters GetPublicKeyParameters()
		{
			if (_key is ECPublicKeyParameters key)
				return key;

            ECPoint q = Secp256k1.G.Multiply(PrivateKey.D);
            return new ECPublicKeyParameters("EC", q, DomainParameter);
        }


		public static ECKey RecoverFromSignature(int recId, ECDSASignature sig, UInt256 message, bool compressed)
		{
			if (recId < 0)
				throw new ArgumentException("recId should be positive");
			if (sig.R.SignValue < 0)
				throw new ArgumentException("r should be positive");
			if (sig.S.SignValue < 0)
				throw new ArgumentException("s should be positive");
			if (message == null)
				throw new ArgumentNullException(nameof(message));


			var curve = ECKey.CreateCurve();

			// 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
			//   1.1 Let x = r + jn

			var n = curve.N;
			var i = BigInteger.ValueOf((long)recId / 2);
			var x = sig.R.Add(i.Multiply(n));

			//   1.2. Convert the integer x to an octet string X of length mlen using the conversion routine
			//        specified in Section 2.3.7, where mlen = ⌈(log2 p)/8⌉ or mlen = ⌈m/8⌉.
			//   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
			//        conversion routine specified in Section 2.3.4. If this conversion routine outputs “invalid”, then
			//        do another iteration of Step 1.
			//
			// More concisely, what these points mean is to use X as a compressed public key.
			var prime = ((FpCurve)curve.Curve).Q;
			if (x.CompareTo(prime) >= 0)
			{
				return null;
			}

			// Compressed keys require you to know an extra bit of data about the y-coord as there are two possibilities.
			// So it's encoded in the recId.
			ECPoint R = DecompressKey(x, (recId & 1) == 1);
			//   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).

			if (!R.Multiply(n).IsInfinity)
				return null;

			//   1.5. Compute e from M using Steps 2 and 3 of ECDSA signature verification.
			var e = new BigInteger(1, message);
			//   1.6. For k from 1 to 2 do the following.   (loop is outside this function via iterating recId)
			//   1.6.1. Compute a candidate public key as:
			//               Q = mi(r) * (sR - eG)
			//
			// Where mi(x) is the modular multiplicative inverse. We transform this into the following:
			//               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
			// Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n). In the above equation
			// ** is point multiplication and + is point addition (the EC group operator).
			//
			// We can find the additive inverse by subtracting e from zero then taking the mod. For example the additive
			// inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and -3 mod 11 = 8.

			var eInv = BigInteger.Zero.Subtract(e).Mod(n);
			var rInv = sig.R.ModInverse(n);
			var srInv = rInv.Multiply(sig.S).Mod(n);
			var eInvrInv = rInv.Multiply(eInv).Mod(n);
			var q = (FpPoint)ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, R, srInv);
			if (compressed)
			{
				q = new FpPoint(curve.Curve, q.X, q.Y, true);
			}
			return new ECKey(q.GetEncoded(), false);
		}

		private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
		{
			var curve = ECKey.CreateCurve().Curve;
			byte[] compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(curve));
			compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
			return curve.DecodePoint(compEnc);
		}

		public static ECKey FromDER(byte[] der)
		{

			// To understand this code, see the definition of the ASN.1 format for EC private keys in the OpenSSL source
			// code in ec_asn1.c:
			//
			// ASN1_SEQUENCE(EC_PRIVATEKEY) = {
			//   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
			//   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
			//   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
			//   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
			// } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
			//

			Asn1InputStream decoder = new Asn1InputStream(der);
			DerSequence seq = (DerSequence)decoder.ReadObject();
			CheckArgument(seq.Count == 4, "Input does not appear to be an ASN.1 OpenSSL EC private key");
			CheckArgument(((DerInteger)seq[0]).Value.Equals(BigInteger.One),
					"Input is of wrong version");
			byte[] bits = ((DerOctetString)seq[1]).GetOctets();
#if !PORTABLE
			decoder.Close();
#else
		decoder.Dispose();
#endif
			return new ECKey(bits, true);
		}

		public static string DumpDer(byte[] der)
		{
			var builder = new StringBuilder();
			var decoder = new Asn1InputStream(der);
			var seq = (DerSequence)decoder.ReadObject();
			builder.AppendLine("Version : " + HexEncoder.Encode(seq[0].GetDerEncoded()));
			builder.AppendLine("Private : " + HexEncoder.Encode(seq[1].GetDerEncoded()));
			builder.AppendLine("Params : " + HexEncoder.Encode(((DerTaggedObject)seq[2]).GetObject().GetDerEncoded()));
			builder.AppendLine("Public : " + HexEncoder.Encode(seq[3].GetDerEncoded()));
			decoder.Close();
			return builder.ToString();
		}

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckArgument(bool predicate, string msg)
		{
			if (!predicate)
			{
				throw new FormatException(msg);
			}
		}

		public byte[] ToDER(bool compressed)
		{
			AssertPrivateKey();
			var baos = new MemoryStream();

			// ASN1_SEQUENCE(EC_PRIVATEKEY) = {
			//   ASN1_SIMPLE(EC_PRIVATEKEY, version, LONG),
			//   ASN1_SIMPLE(EC_PRIVATEKEY, privateKey, ASN1_OCTET_STRING),
			//   ASN1_EXP_OPT(EC_PRIVATEKEY, parameters, ECPKPARAMETERS, 0),
			//   ASN1_EXP_OPT(EC_PRIVATEKEY, publicKey, ASN1_BIT_STRING, 1)
			// } ASN1_SEQUENCE_END(EC_PRIVATEKEY)
			var seq = new DerSequenceGenerator(baos);
			seq.AddObject(new DerInteger(1)); // version
			seq.AddObject(new DerOctetString(PrivateKey.D.ToByteArrayUnsigned()));


			//Did not managed to generate the same der as brainwallet by using this
			//seq.AddObject(new DerTaggedObject(0, Secp256k1.ToAsn1Object()));
            var secp256k1Der = Asn1Object.FromByteArray(compressed ? HexEncoder.Decode("308182020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704210279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101") : HexEncoder.Decode("3081a2020101302c06072a8648ce3d0101022100fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f300604010004010704410479be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8022100fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141020101"));
            seq.AddObject(new DerTaggedObject(0, secp256k1Der));
			seq.AddObject(new DerTaggedObject(1, new DerBitString(GetPublicKeyPoint(compressed))));
			seq.Close();
			return baos.ToArray();
		}
	}
}
