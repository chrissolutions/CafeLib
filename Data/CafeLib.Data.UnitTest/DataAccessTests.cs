using System;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Data.Extensions;
using CafeLib.Data.UnitTest.ChequeAccess;
using Xunit;

namespace CafeLib.Data.UnitTest
{
    public class DataAccessTests
    {
        private readonly ChequeDatabase _chequeDatabase;

        public DataAccessTests()
        {
            _chequeDatabase = new ChequeDatabase();
        }

        [Fact]
        public async Task Mapping_Enum_To_String_Test()
        {
            var storage = _chequeDatabase.GetStorage();
            var cheques = (await storage.Find<Cheque>(x => x.Status == ChequeStatus.Uploaded)).ToArray();
            Assert.NotNull(cheques);
            Assert.NotEmpty(cheques);
            Assert.Equal(47, cheques.Length);
        }

        [Fact]
        public async Task Mapping_Find_By_CreationDate_Test()
        {
            var storage = _chequeDatabase.GetStorage();
            var creationDate = new DateTime(2020, 02, 27);
            var cheques = (await storage.FindAll<Cheque>())
                .Where(x => x.CreationDate.Date == creationDate).ToArray();
            Assert.NotNull(cheques);
            Assert.NotEmpty(cheques);
            Assert.Equal(9, cheques.Length);
        }
    }
}
