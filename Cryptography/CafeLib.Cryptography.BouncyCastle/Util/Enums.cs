using System;
using System.Reflection;
using CafeLib.Cryptography.BouncyCastle.Util.Date;

namespace CafeLib.Cryptography.BouncyCastle.Util
{
	internal abstract class Enums
	{
		internal static Enum GetEnumValue(System.Type enumType, string s)
		{
			if(!enumType.GetTypeInfo().IsEnum)
				throw new ArgumentException("Not an enumeration type", nameof(enumType));

			// We only want to parse single named constants
			if (s.Length <= 0 || !char.IsLetter(s[0]) || s.IndexOf(',') >= 0) throw new ArgumentException();
			s = s.Replace('-', '_');
			s = s.Replace('/', '_');
			return (Enum)Enum.Parse(enumType, s, false);

		}

		internal static Array GetEnumValues(System.Type enumType)
		{
			if(!enumType.GetTypeInfo().IsEnum)
				throw new ArgumentException("Not an enumeration type", nameof(enumType));

			return Enum.GetValues(enumType);
		}

		internal static Enum GetArbitraryValue(System.Type enumType)
		{
			Array values = GetEnumValues(enumType);
			int pos = (int)(DateTimeUtilities.CurrentUnixMs() & int.MaxValue) % values.Length;
			return (Enum)values.GetValue(pos);
		}
	}
}
