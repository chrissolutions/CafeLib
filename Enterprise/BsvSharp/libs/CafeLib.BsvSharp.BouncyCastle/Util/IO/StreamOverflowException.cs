using System;
using System.IO;

namespace CafeLib.BsvSharp.BouncyCastle.Util.IO
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT)
    [Serializable]
#endif
    public class StreamOverflowException
		: IOException
	{
		public StreamOverflowException()
			: base()
		{
		}

		public StreamOverflowException(
			string message)
			: base(message)
		{
		}

		public StreamOverflowException(
			string		message,
			Exception	exception)
			: base(message, exception)
		{
		}
	}
}
