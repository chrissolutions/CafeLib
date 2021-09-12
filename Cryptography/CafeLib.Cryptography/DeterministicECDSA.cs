using System;
using System.Text;
using CafeLib.Cryptography.BouncyCastle.Crypto;
using CafeLib.Cryptography.BouncyCastle.Crypto.Parameters;
using CafeLib.Cryptography.BouncyCastle.Math;
using CafeLib.Cryptography.BouncyCastle.Security;

namespace CafeLib.Cryptography
{
    // ==================================================================
	//http://tools.ietf.org/html/rfc6979#appendix-A
	/**
	 * Deterministic DSA signature generation.  This is a sample
	 * implementation designed to illustrate how deterministic DSA
	 * chooses the pseudorandom value k when signing a given message.
	 * This implementation was NOT optimized or hardened against
	 * side-channel leaks.
	 *
	 * An instance is created with a hash function name, which must be
	 * supported by the underlying Java virtual machine ("SHA-1" and
	 * "SHA-256" should work everywhere).  The data to sign is input
	 * through the {@code update()} methods.  The private key is set with
	 * {@link #setPrivateKey}.  The signature is obtained by calling
	 * {@link #sign}; alternatively, {@link #signHash} can be used to
	 * sign some data that has been externally hashed.  The private key
	 * MUST be set before generating the signature itself, but message
	 * data can be input before setting the key.
	 *
	 * Instances are NOT thread-safe.  However, once a signature has
	 * been generated, the same instance can be used again for another
	 * signature; {@link #setPrivateKey} need not be called again if the
	 * private key has not changed.  {@link #reset} can also be called to
	 * cancel previously input data.  Generating a signature with {@link
	 * #sign} (not {@link #signHash}) also implicitly causes a
	 * reset.
	 *
	 * ------------------------------------------------------------------
	 * Copyright (c) 2013 IETF Trust and the persons identified as
	 * authors of the code.  All rights reserved.
	 *
	 * Redistribution and use in source and binary forms, with or without
	 * modification, is permitted pursuant to, and subject to the license
	 * terms contained in, the Simplified BSD License set forth in Section
	 * 4.c of the IETF Trust's Legal Provisions Relating to IETF Documents
	 * (http://trustee.ietf.org/license-info).
	 *
	 * Technical remarks and questions can be addressed to:
	 * pornin@bolet.org
	 * ------------------------------------------------------------------
	 */

	internal class DeterministicECDSA
	{
        private readonly IDigest _dig;
		private readonly IMac _hmac;
		private BigInteger _p, _q, _g, _x;
        private int _qlen, _rlen, _rolen;
        private readonly int _holen;
		private byte[] _bx;
		private ECDomainParameters _curve;

		public DeterministicECDSA()
			: this("SHA-256")
		{
		}
		/**
		 * Create an instance, using the specified hash function.
		 * The name is used to obtain from the JVM an implementation
		 * of the hash function and an implementation of HMAC.
		 *
		 * @param hashName   the hash function name
		 * @throws IllegalArgumentException  on unsupported name
		 */
		public DeterministicECDSA(string hashName)
		{
            string macName;
            try
			{
				_dig = DigestUtilities.GetDigest(hashName);
			}
			catch (SecurityUtilityException nsae)
			{
				throw new ArgumentException("Invalid hash", nameof(hashName), nsae);
			}
			if (hashName.IndexOf('-') < 0)
			{
				macName = "Hmac" + hashName;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Hmac");
				int n = hashName.Length;
				for (int i = 0; i < n; i++)
				{
					char c = hashName[i];
					if (c != '-')
					{
						sb.Append(c);
					}
				}
				macName = sb.ToString();

			}
			try
			{
				_hmac = MacUtilities.GetMac(macName);
			}
			catch (SecurityUtilityException nsae)
			{
				throw new InvalidOperationException(nsae.Message, nsae);
			}
			_holen = _hmac.GetMacSize();
		}

		/**
		 * Set the private key.
		 *
		 * @param p   key parameter: field modulus
		 * @param q   key parameter: subgroup order
		 * @param g   key parameter: generator
		 * @param x   private key
		 */
		public void SetPrivateKey(BigInteger p, BigInteger q,
				BigInteger g, BigInteger x)
		{
			/*
			 * Perform some basic sanity checks.  We do not
			 * check primality of p or q because that would
			 * be too expensive.
			 *
			 * We reject keys where q is longer than 999 bits,
			 * because it would complicate signature encoding.
			 * Normal DSA keys do not have a q longer than 256
			 * bits anyway.
			 */
			if (p == null || q == null || g == null || x == null
					|| p.SignValue <= 0 || q.SignValue <= 0
					|| g.SignValue <= 0 || x.SignValue <= 0
					|| x.CompareTo(q) >= 0 || q.CompareTo(p) >= 0
					|| q.BitLength > 999
					|| g.CompareTo(p) >= 0 || g.BitLength == 1
					|| g.ModPow(q, p).BitLength != 1)
			{
				throw new InvalidOperationException(
						"invalid DSA private key");
			}
			this._p = p;
			this._q = q;
			this._g = g;
			this._x = x;
			_qlen = q.BitLength;
			if (q.SignValue <= 0 || _qlen < 8)
			{
				throw new InvalidOperationException(
						"bad group order: " + q);

			}
			_rolen = (_qlen + 7) >> 3;
			_rlen = _rolen * 8;

			/*
			 * Convert the private exponent (x) into a sequence
			 * of octets.
			 */
			_bx = Int2Octets(x);
		}
		public void SetPrivateKey(ECPrivateKeyParameters ecKey)
		{
			this._x = ecKey.D;
			this._q = ecKey.Parameters.N;
			this._curve = ecKey.Parameters;

			_qlen = _q.BitLength;
			if (_q.SignValue <= 0 || _qlen < 8)
			{
				throw new InvalidOperationException(
						"bad group order: " + _q);

			}
			_rolen = (_qlen + 7) >> 3;
			_rlen = _rolen * 8;
			_bx = Int2Octets(_x);
		}
		private BigInteger Bits2Int(byte[] @in)
		{
			BigInteger v = new BigInteger(1, @in);
			int vlen = @in.Length * 8;
			if (vlen > _qlen)
			{
				v = v.ShiftRight(vlen - _qlen);
			}
			return v;
		}

		private byte[] Int2Octets(BigInteger v)
		{
			byte[] @out = v.ToByteArray();
			if (@out.Length < _rolen)
			{
				byte[] out2 = new byte[_rolen];
				Array.Copy(@out, 0,
						out2, _rolen - @out.Length,
						@out.Length);
				return out2;
			}
			else if (@out.Length > _rolen)
			{
				byte[] out2 = new byte[_rolen];
				Array.Copy(@out, @out.Length - _rolen,
						out2, 0, _rolen);
				return out2;
			}
			else
			{
				return @out;
			}
		}

		private byte[] Bits2Octets(byte[] @in)
		{
			BigInteger z1 = Bits2Int(@in);
			BigInteger z2 = z1.Subtract(_q);
			return Int2Octets(z2.SignValue < 0 ? z1 : z2);
		}

		/**
		 * Set (or reset) the secret key used for HMAC.
		 *
		 * @param k   the new secret key
		 */
		private void SetHmacKey(byte[] k)
		{
			try
			{
				_hmac.Init(new KeyParameter(k));
			}
			catch (InvalidKeyException ike)
			{
				throw new InvalidOperationException(ike.Message, ike);
			}
		}

		/**
		 * Compute the pseudorandom k for signature generation,
		 * using the process specified for deterministic DSA.
		 *
		 * @param h1   the hashed message
		 * @return  the pseudorandom k to use
		 */
		private BigInteger ComputeK(byte[] h1)
		{
			/*
			 * Convert hash value into an appropriately truncated
			 * and/or expanded sequence of octets.  The private
			 * key was already processed (into field bx[]).
			 */
			byte[] bh = Bits2Octets(h1);

			/*
			 * HMAC is always used with K as key.
			 * Whenever K is updated, we reset the
			 * current HMAC key.
			 */

			/* step b. */
			byte[] V = new byte[_holen];
			for (int i = 0; i < _holen; i++)
			{
				V[i] = 0x01;
			}

			/* step c. */
			byte[] K = new byte[_holen];
			SetHmacKey(K);

			/* step d. */
			_hmac.Update(V);
			_hmac.Update((byte)0x00);

			_hmac.Update(_bx);
			_hmac.Update(bh);
			K = _hmac.DoFinal();
			SetHmacKey(K);

			/* step e. */
			_hmac.Update(V);
			V = _hmac.DoFinal();

			/* step f. */
			_hmac.Update(V);
			_hmac.Update((byte)0x01);
			_hmac.Update(_bx);
			_hmac.Update(bh);
			K = _hmac.DoFinal();
			SetHmacKey(K);

			/* step g. */
			_hmac.Update(V);
			V = _hmac.DoFinal();

			/* step h. */
			byte[] T = new byte[_rolen];
			for (; ; )
			{
				/*
				 * We want qlen bits, but we support only
				 * hash functions with an output length
				 * multiple of 8;acd hence, we will gather
				 * rlen bits, i.e., rolen octets.
				 */
				int toff = 0;
				while (toff < _rolen)
				{
					_hmac.Update(V);
					V = _hmac.DoFinal();
					int cc = Math.Min(V.Length,
							T.Length - toff);
					Array.Copy(V, 0, T, toff, cc);
					toff += cc;
				}
				BigInteger k = Bits2Int(T);
				if (k.SignValue > 0 && k.CompareTo(_q) < 0)
				{
					return k;
				}

				/*
				 * k is not in the proper range; update
				 * K and V, and loop.
				 */

				_hmac.Update(V);
				_hmac.Update((byte)0x00);
				K = _hmac.DoFinal();
				SetHmacKey(K);
				_hmac.Update(V);
				V = _hmac.DoFinal();
			}
		}

		/**
		 * Process one more byte of input data (message to sign).
		 *
		 * @param in   the extra input byte
		 */
		public void Update(byte @in)
		{
			_dig.Update(@in);
		}

		/**
		 * Process some extra bytes of input data (message to sign).
		 *
		 * @param in   the extra input bytes
		 */
		public void Update(byte[] @in)
		{
			_dig.Update(@in, 0, @in.Length);
		}

		/**
		 * Process some extra bytes of input data (message to sign).
		 *
		 * @param in    the extra input buffer
		 * @param off   the extra input offset
		 * @param len   the extra input length (in bytes)
		 */
		public void Update(byte[] @in, int off, int len)
		{
			_dig.Update(@in, off, len);
		}

		/**
		 * Produce the signature.  {@link #setPrivateKey} MUST have
		 * been called.  The signature is computed over the data
		 * that was input through the {@code update*()} methods.
		 * This engine is then reset (made ready for a new
		 * signature generation).
		 *
		 * @return  the signature
		 */
		public byte[] Sign()
		{
			return SignHash(_dig.Digest());
		}

		/**
		 * Produce the signature.  {@link #setPrivateKey} MUST
		 * have been called.  The signature is computed over the
		 * provided hash value (data is assumed to have been hashed
		 * externally).  The data that was input through the
		 * {@code update*()} methods is ignored, but kept.
		 *
		 * If the hash output is longer than the subgroup order
		 * (the length of q, in bits, denoted 'qlen'), then the
		 * provided value {@code h1} can be truncated, provided that
		 * at least qlen leading bits are preserved.  In other words,
		 * bit values in {@code h1} beyond the first qlen bits are
		 * ignored.
		 *
		 * @param h1   the hash value
		 * @return  the signature
		 */
		public byte[] SignHash(byte[] h1)
		{
			if (_p == null && _curve == null)
			{
				throw new InvalidOperationException(
						"no private key set");
			}
			try
			{
				BigInteger k = ComputeK(h1);
				BigInteger r = _curve == null ?
					_g.ModPow(k, _p).Mod(_q) :
					_curve.G.Multiply(k).X.ToBigInteger().Mod(_q);
				BigInteger s = k.ModInverse(_q).Multiply(
						Bits2Int(h1).Add(_x.Multiply(r)))
						.Mod(_q);

				/*
				 * Signature encoding: ASN.1 SEQUENCE of
				 * two INTEGERs.  The conditions on q
				 * imply that the encoded version of r and
				 * s is no longer than 127 bytes for each,
				 * including DER tag and length.
				 */
				byte[] br = r.ToByteArray();
				byte[] bs = s.ToByteArray();
				int ulen = br.Length + bs.Length + 4;
				int slen = ulen + (ulen >= 128 ? 3 : 2);

				byte[] sig = new byte[slen];
				int i = 0;
				sig[i++] = 0x30;
				if (ulen >= 128)
				{
					sig[i++] = (byte)0x81;
					sig[i++] = (byte)ulen;
				}
				else
				{
					sig[i++] = (byte)ulen;
				}
				sig[i++] = 0x02;
				sig[i++] = (byte)br.Length;
				Array.Copy(br, 0, sig, i, br.Length);
				i += br.Length;
				sig[i++] = 0x02;
				sig[i++] = (byte)bs.Length;
				Array.Copy(bs, 0, sig, i, bs.Length);
				LastK = k;
				LastR = r;
				return sig;

			}
			catch (ArithmeticException ae)
			{
				throw new InvalidOperationException(
						"DSA error (bad key ?)", ae);
			}
		}

		public BigInteger LastK
		{
			get;
			set;
		}
		public BigInteger LastR
		{
			get;
			set;
		}

		/**
		 * Reset this engine.  Data input through the {@code
		 * update*()} methods is discarded.  The current private key,
		 * if one was set, is kept unchanged.
		 */
		public void Reset()
		{
			_dig.Reset();
		}
	}
}
