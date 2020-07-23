namespace CafeLib.Aspnet.Identity.Secrets
{
	public interface IPasswordHash
	{		
		string HashPassword(string password);
		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
