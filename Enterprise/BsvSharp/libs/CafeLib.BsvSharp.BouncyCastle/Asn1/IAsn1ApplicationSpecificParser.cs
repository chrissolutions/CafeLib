namespace CafeLib.BsvSharp.BouncyCastle.Asn1
{
	public interface IAsn1ApplicationSpecificParser
    	: IAsn1Convertible
	{
    	IAsn1Convertible ReadObject();
	}
}
