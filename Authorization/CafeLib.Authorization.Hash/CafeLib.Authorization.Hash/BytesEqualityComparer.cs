using System;
using System.Collections.Generic;
using CafeLib.Core.Extensions;

namespace CafeLib.Authorization.Hash
{
	internal class BytesEqualityComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] array1, byte[] array2)
        {
            switch (array1, array2)
            {
				case var _ when ReferenceEquals(array1, array2):
                    return true;

				case var _ when array1?.Length != array2?.Length:
                    return false;

				default:
                    return array1.Every((b, i) => b == array2[i]);
            }
        }

		public override int GetHashCode(byte[] obj)
		{
			var result = Convert.ToBase64String(obj ?? Array.Empty<byte>());
			return result.GetHashCode();
		}
	}
}
