using CafeLib.Data.Persistence;
using CafeLib.Data.Sources;

namespace CafeLib.Data.UnitTest.ChequeAccess
{
    public class ChequeStorage : Storage<ChequeDomain>
    {
        public ChequeStorage(string connectionUri, IConnectionOptions options)
            : base(connectionUri, options)
        {
        }
    }
}