using CafeLib.Data.Persistence;
using CafeLib.Data.Sources;

namespace CafeLib.Authorization.Identity
{
    public class IdentityStorage : Storage<IdentityDomain>
    {
        public IdentityStorage(string connectionUri, IConnectionOptions options) 
            : base(connectionUri, options)
        {
        }
    }
}
