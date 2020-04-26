using CafeLib.Data.Persistence;
using CafeLib.Data.Sources;

namespace AtmAgent.Cheques
{
    public class ChequeStorage : Storage<ChequeDomain>
    {
        public ChequeStorage(string connectionUri, IConnectionOptions options)
            : base(connectionUri, options)
        {
        }
    }
}
