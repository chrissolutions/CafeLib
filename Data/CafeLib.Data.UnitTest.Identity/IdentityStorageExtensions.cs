using System.Globalization;
using System.Threading.Tasks;
using CafeLib.Data.Extensions;
using CafeLib.Data.Scripts;
using RepoDb;

namespace CafeLib.Data.UnitTest.Identity
{
    public static class IdentityStorageExtensions
    {
        public static async Task<T> FindUserByEmailAddress<T>(this IdentityStorage storage, string emailAddress) where T : IdentityUser
        {
            var normalizedEmail = emailAddress.ToUpper(CultureInfo.CurrentCulture);
            return await storage.FindOne<T>(x => x.NormalizedEmail == normalizedEmail);
        }

        public static async Task<T> FindUserByUserName<T>(this IdentityStorage storage, string userName) where T : IdentityUser
        {
            var normalizedUserName = userName.ToUpper(CultureInfo.CurrentCulture);
            return await storage.FindOne<T>(x => x.NormalizedUserName == normalizedUserName);
        }

        public static void CreateDatabase(this IdentityStorage storage)
        {
            var script = Script.GetScript("IdentityDb.sql");
            using var connection = storage.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            connection.ExecuteNonQuery(script);
            transaction.Commit();
        }
    }
}