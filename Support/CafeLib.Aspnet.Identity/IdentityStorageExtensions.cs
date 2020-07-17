using System.Globalization;
using System.Threading.Tasks;
using CafeLib.Data.Extensions;

namespace CafeLib.Aspnet.Identity
{
    public static class IdentityStorageExtensions
    {
        public static async Task<T> FindUserByEmailAddress<T>(this IdentityStorage storage, string emailAddress) where T : IdentityUser
        {
            return await storage.FindOne<T>(x => x.NormalizedEmail.ToLower(CultureInfo.CurrentCulture) == emailAddress.ToLower(CultureInfo.CurrentCulture));
        }

        public static async Task<T> FindUserByUserName<T>(this IdentityStorage storage, string userName) where T : IdentityUser
        {
            return await storage.FindOne<T>(x => x.NormalizedUserName.ToLower(CultureInfo.CurrentCulture) == userName.ToLower(CultureInfo.CurrentCulture));
        }
    }
}
