using System;
using System.Collections.Generic;

namespace CafeLib.Aspnet.Identity.Secrets
{
	public class BytesEqualityComparer : EqualityComparer<byte[]>
	{
		public override bool Equals(byte[] x, byte[] y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null || x.Length != y.Length)
			{
				return false;
			}

			for (int i = 0; i < x.Length; i++)
			{
				if (x[i] != y[i])
					return false;
			}

			return true;
		}

		public override int GetHashCode(byte[] obj)
		{
			var result = Convert.ToBase64String(obj ?? Array.Empty<byte>());
			return result.GetHashCode();
		}

	}
}
