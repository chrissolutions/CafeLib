using System;
using System.Linq;
using System.Threading.Tasks;
using CafeLib.Data.Extensions;
using CafeLib.Data.UnitTest.ChequeAccess;
using CafeLib.Data.UnitTest.Identity;
using CafeLib.Data.UnitTest.IdentityAccess;
using CafeLib.Data.UnitTest.Models;
using Xunit;

namespace CafeLib.Data.UnitTest
{
    public class DataAccessTests
    {
        private readonly IdentityDatabase _identityDatabase;
        private readonly ChequeDatabase _chequeDatabase;

        public DataAccessTests()
        {
            _identityDatabase = new IdentityDatabase();
            _chequeDatabase = new ChequeDatabase();
        }

        [Fact]
        public async Task IdentityUser_Test()
        {
            var login = new LoginModel
            {
                UserName = "alice",
                Password = "My long 123$ password",
                EmailAddress = "AliceSmith@email.com",
                RememberMe = true
            };

            var storage = _identityDatabase.GetStorage();
            var user = await storage.FindUserByUserName<IdentityUser>(login.UserName);
            Assert.NotNull(user);
            Assert.Equal(login.UserName, user.UserName);
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
