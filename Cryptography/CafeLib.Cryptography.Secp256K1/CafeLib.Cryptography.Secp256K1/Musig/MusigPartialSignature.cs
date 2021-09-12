#if HAS_SPAN
#nullable enable
using System;

namespace CafeLib.Cryptography.Secp256K1.Musig
{
#if SECP256K1_LIB
	public
#endif
	class MusigPartialSignature
	{
#if SECP256K1_LIB
		public
#else
		internal
#endif
		readonly Scalar E;

		public MusigPartialSignature(Scalar e)
		{
			this.E = e;
		}

		public MusigPartialSignature(ReadOnlySpan<byte> in32)
		{
			this.E = new Scalar(in32, out var overflow);
			if (overflow != 0)
				throw new ArgumentOutOfRangeException(nameof(in32), "in32 is overflowing");
		}

		public void WriteToSpan(Span<byte> in32)
		{
			E.WriteToSpan(in32);
		}
		public byte[] ToBytes()
		{
			byte[] b = new byte[32];
			WriteToSpan(b);
			return b;
		}		
	}
}
#endif
