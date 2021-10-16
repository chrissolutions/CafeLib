using CafeLib.Data.Persistence;
using CafeLib.Data.Sources;

namespace CafeLib.Data.UnitTest.TestDomain
{
    public class TestStorage : Storage<TestDomain>
    {
        public TestStorage(string connectionUri) 
            : base(connectionUri, new SqlOptions())
        {
        }
    }
}
