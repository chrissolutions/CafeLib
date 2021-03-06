using System;

namespace CafeLib.Cryptography.BouncyCastle.Security
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT)
    [Serializable]
#endif
    public class InvalidParameterException : KeyException
	{
		public InvalidParameterException() : base() { }
		public InvalidParameterException(string message) : base(message) { }
		public InvalidParameterException(string message, Exception exception) : base(message, exception) { }
	}
}
