using CafeLib.Data.Persistence;
using CafeLib.Data.Sources.Sqlite;

namespace AtmAgent.Cheques
{
    public class ChequeStorage : Storage<ChequeDomain>
    {
        public ChequeStorage(string connectionUri)
            : base(connectionUri, new SqliteOptions())
        {
        }
    }
}
