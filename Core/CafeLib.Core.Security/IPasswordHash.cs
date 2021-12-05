using System;

namespace CafeLib.Core.Security
{
    [Obsolete("Deprecated.  Use 'CafeLib.Authorization.Security' assembly instead.")]
	public interface IPasswordHash
	{		
		string HashPassword(string password);
		bool VerifyHashedPassword(string hashedPassword, string providedPassword);
	}
}
