using System.IO;

namespace CafeLib.BsvSharp.BouncyCastle.Asn1
{
	public interface Asn1OctetStringParser
		: IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
