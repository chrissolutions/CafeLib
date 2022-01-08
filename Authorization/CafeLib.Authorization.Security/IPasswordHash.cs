namespace CafeLib.Authorization.Security
{
    public interface IPasswordHash
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}