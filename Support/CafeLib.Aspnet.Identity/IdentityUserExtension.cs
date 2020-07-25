using CafeLib.Aspnet.Identity.Secrets;

namespace CafeLib.Aspnet.Identity
{
    public static class IdentityUserExtension
    {
        public static bool VerifyPassword(this IdentityUser user, string password)
        {
            return PasswordHash.Default.VerifyHashedPassword(user.PasswordHash, password);
        }
    }
}
