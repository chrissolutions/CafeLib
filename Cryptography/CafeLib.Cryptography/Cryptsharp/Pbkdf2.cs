﻿#region License
/*
CryptSharp
Copyright (c) 2011, 2013 James F. Bellinger <http://www.zer7.com/software/cryptsharp>

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
#endregion

using System;
using System.IO;
using CafeLib.Cryptography.BouncyCastle.Crypto;

namespace CafeLib.Cryptography.Cryptsharp
{
	/// <summary>
	/// Implements the PBKDF2 key derivation function.
	/// </summary>
	/// 
	/// <example>
	/// <code title="Computing a Derived Key">
	/// 
	/// // Compute a 128-byte derived key using HMAC-SHA256, 1000 iterations, and a given key and salt.
	/// byte[] derivedKey = Pbkdf2.ComputeDerivedKey(new HMACSHA256(key), salt, 1000, 128);
	/// </code>
	/// <code title="Creating a Derived Key Stream">
	///
	/// // Create a stream using HMAC-SHA512, 1000 iterations, and a given key and salt.
	/// Stream derivedKeyStream = new Pbkdf2(new HMACSHA512(key), salt, 1000);
	/// </code>
	/// </example>
	public class Pbkdf2 : Stream
	{
		#region PBKDF2

        private readonly byte[] _saltBuffer;
        private readonly byte[] _digest;
        private readonly byte[] _digestT1;

        private readonly IMac _hmacAlgorithm;
        private readonly int _iterations;

		/// <summary>
		/// Creates a new PBKDF2 stream.
		/// </summary>
		/// <param name="hmacAlgorithm">
		/// </param>
		/// <param name="salt">
		///     The salt.
		///     A unique salt means a unique PBKDF2 stream, even if the original key is identical.
		/// </param>
		/// <param name="iterations">The number of iterations to apply.</param>
		internal Pbkdf2(IMac hmacAlgorithm, byte[] salt, int iterations)
		{
			Check.Null("hmacAlgorithm", hmacAlgorithm);
			Check.Null("salt", salt);
			Check.Length("salt", salt, 0, int.MaxValue - 4);
			Check.Range("iterations", iterations, 1, int.MaxValue);
			int hmacLength = hmacAlgorithm.GetMacSize();
			_saltBuffer = new byte[salt.Length + 4];
			Array.Copy(salt, _saltBuffer, salt.Length);
			_iterations = iterations;
			_hmacAlgorithm = hmacAlgorithm;
			_digest = new byte[hmacLength];
			_digestT1 = new byte[hmacLength];
		}

		/// <summary>
		/// Reads from the derived key stream.
		/// </summary>
		/// <param name="count">The number of bytes to read.</param>
		/// <returns>Bytes from the derived key stream.</returns>
		public byte[] Read(int count)
		{
			Check.Range("count", count, 0, int.MaxValue);

			var buffer = new byte[count];
			var bytes = Read(buffer, 0, count);
			if (bytes < count)
			{
				throw Exceptions.Argument("count", "Can only return {0} bytes.", bytes);
			}

			return buffer;
		}

		/// <summary>
		/// Computes a derived key.
		/// </summary>
		/// <param name="hmacAlgorithm">
		/// </param>
		/// <param name="salt">
		///     The salt.
		///     A unique salt means a unique derived key, even if the original key is identical.
		/// </param>
		/// <param name="iterations">The number of iterations to apply.</param>
		/// <param name="derivedKeyLength">The desired length of the derived key.</param>
		/// <returns>The derived key.</returns>
		public static byte[] ComputeDerivedKey(IMac hmacAlgorithm, byte[] salt, int iterations, int derivedKeyLength)
		{
			Check.Range("derivedKeyLength", derivedKeyLength, 0, int.MaxValue);

            using var kdf = new Pbkdf2(hmacAlgorithm, salt, iterations);
            return kdf.Read(derivedKeyLength);
        }

		/// <summary>
		/// Closes the stream, clearing memory and disposing of the HMAC algorithm.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			Security.Clear(_saltBuffer);
			Security.Clear(_digest);
			Security.Clear(_digestT1);

			DisposeHmac();
		}

		private void DisposeHmac()
		{
			_hmacAlgorithm.Reset();
		}

		void ComputeBlock(uint pos)
		{
			BitPacking.BEBytesFromUInt32(pos, _saltBuffer, _saltBuffer.Length - 4);
			ComputeHmac(_saltBuffer, _digestT1);
			Array.Copy(_digestT1, _digest, _digestT1.Length);

			for (int i = 1; i < _iterations; i++)
			{
				ComputeHmac(_digestT1, _digestT1);
				for (int j = 0; j < _digest.Length; j++)
				{
					_digest[j] ^= _digestT1[j];
				}
			}

			Security.Clear(_digestT1);
		}

		void ComputeHmac(byte[] input, byte[] output)
		{
			var hash = new byte[_hmacAlgorithm.GetMacSize()];
			_hmacAlgorithm.BlockUpdate(input, 0, input.Length);
			_hmacAlgorithm.DoFinal(hash, 0);
			Array.Copy(hash, output, output.Length);
		}

		#endregion

		#region Stream
		long _blockStart, _blockEnd, _pos;

		/// <exclude />
		public override void Flush()
		{

		}

		/// <inheritdoc />
		public override int Read(byte[] buffer, int offset, int count)
		{
			Check.Bounds("buffer", buffer, offset, count);
			int bytes = 0;

			while (count > 0)
			{
				if (Position < _blockStart || Position >= _blockEnd)
				{
					if (Position >= Length)
					{
						break;
					}

					long pos = Position / _digest.Length;
					ComputeBlock((uint)(pos + 1));
					_blockStart = pos * _digest.Length;
					_blockEnd = _blockStart + _digest.Length;
				}

				int bytesSoFar = (int)(Position - _blockStart);
				int bytesThisTime = Math.Min(_digest.Length - bytesSoFar, count);
				Array.Copy(_digest, bytesSoFar, buffer, bytes, bytesThisTime);
				count -= bytesThisTime;
				bytes += bytesThisTime;
				Position += bytesThisTime;
			}

			return bytes;
		}

		/// <inheritdoc />
		public override long Seek(long offset, SeekOrigin origin)
		{
			long pos;

			switch (origin)
			{
				case SeekOrigin.Begin:
					pos = offset;
					break;
				case SeekOrigin.Current:
					pos = Position + offset;
					break;
				case SeekOrigin.End:
					pos = Length + offset;
					break;
				default:
					throw Exceptions.ArgumentOutOfRange("origin", "Unknown seek type.");
			}

			if (pos < 0)
			{
				throw Exceptions.Argument("offset", "Can't seek before the stream start.");
			}
			Position = pos;
			return pos;
		}

		/// <exclude />
		public override void SetLength(long value)
		{
			throw Exceptions.NotSupported();
		}

		/// <exclude />
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw Exceptions.NotSupported();
		}

		/// <exclude />
		public override bool CanRead => true;

        /// <exclude />
		public override bool CanSeek => true;

        /// <exclude />
		public override bool CanWrite => false;

        /// <summary>
		/// The maximum number of bytes that can be derived is 2^32-1 times the HMAC size.
		/// </summary>
		public override long Length => _digest.Length * uint.MaxValue;

        /// <summary>
		/// The position within the derived key stream.
		/// </summary>
		public override long Position
		{
			get => _pos;
            set
			{
				if (_pos < 0)
				{
					throw Exceptions.Argument(null, "Can't seek before the stream start.");
				}
				_pos = value;
			}
		}
		#endregion
	}
}
