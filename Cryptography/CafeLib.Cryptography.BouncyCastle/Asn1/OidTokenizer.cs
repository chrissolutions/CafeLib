namespace CafeLib.Cryptography.BouncyCastle.Asn1
{
    /**
     * class for breaking up an Oid into it's component tokens, ala
     * java.util.StringTokenizer. We need this class as some of the
     * lightweight Java environment don't support classes like
     * StringTokenizer.
     */
    public class OidTokenizer
    {
        private readonly string _oid;
        private int _index;

		public OidTokenizer(string oid)
        {
            _oid = oid;
        }

		public bool HasMoreTokens
        {
			get { return _index != -1; }
        }

		public string NextToken()
        {
            if (_index == -1)
            {
                return null;
            }

            int end = _oid.IndexOf('.', _index);
            if (end == -1)
            {
                string lastToken = _oid[_index..];
                _index = -1;
                return lastToken;
            }

            string nextToken = _oid[_index..end];
			_index = end + 1;
            return nextToken;
        }
    }
}
