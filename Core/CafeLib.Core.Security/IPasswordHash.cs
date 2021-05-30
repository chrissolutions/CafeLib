namespace CafeLib.Core.Hashing
{
	public interface IPasswordHash
	{		
		string HashPassword(string password);
		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
