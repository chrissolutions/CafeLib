namespace CafeLib.Authorization.Hash
{
	public interface IPasswordHash
	{		
		string HashPassword(string password);
		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
