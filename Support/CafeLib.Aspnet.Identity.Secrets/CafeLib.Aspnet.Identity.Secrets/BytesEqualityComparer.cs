using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeLib.Aspnet.Identity.Secrets
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
                    var index = 0;
					return array1.All(b => b == array2[index++]);
            }
        }

		public override int GetHashCode(byte[] obj)
		{
			var result = Convert.ToBase64String(obj ?? Array.Empty<byte>());
			return result.GetHashCode();
		}
	}
}
