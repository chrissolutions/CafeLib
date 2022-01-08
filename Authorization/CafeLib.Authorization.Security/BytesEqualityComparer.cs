using System;
using System.Collections.Generic;
using CafeLib.Core.Extensions;

namespace CafeLib.Authorization.Security
{
    internal class BytesEqualityComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] array1, byte[] array2)
        {
            return (array1, array2) switch
            {
                _ when ReferenceEquals(array1, array2) => true,
                _ when array1?.Length != array2?.Length => false,
                _ => array1.Every((b, i) => b == array2[i])
            };
        }

        public override int GetHashCode(byte[] obj)
        {
            var result = Convert.ToBase64String(obj);
            return result.GetHashCode();
        }
    }
}