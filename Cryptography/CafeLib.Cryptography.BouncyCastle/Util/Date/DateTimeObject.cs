using System;

namespace CafeLib.Cryptography.BouncyCastle.Util.Date
{
	public sealed class DateTimeObject
	{
		private readonly DateTime dt;

		public DateTimeObject(
			DateTime dt)
		{
			this.dt = dt;
		}

		public DateTime Value
		{
			get { return dt; }
		}

		public override string ToString()
		{
			return dt.ToString();
		}
	}
}
