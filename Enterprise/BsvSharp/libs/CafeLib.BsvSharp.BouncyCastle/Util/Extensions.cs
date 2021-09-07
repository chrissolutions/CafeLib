using System;
using System.Reflection;

namespace CafeLib.BsvSharp.BouncyCastle.Util
{
	public static class Extensions
	{
		public static bool IsInstanceOfType(this Type type, object obj)
		{
			return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
		}
	}
}
