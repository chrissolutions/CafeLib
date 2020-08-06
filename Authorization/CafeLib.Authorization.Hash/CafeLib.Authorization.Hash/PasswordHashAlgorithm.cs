﻿namespace CafeLib.Authorization.Hash
{
	public enum PasswordHashAlgorithm
	{		
		//MD5 = 1, Not supported
		Sha1 = 2, 
		Sha256 = 3, 
		Sha384 = 4, 
		Sha512 = 5, 
	}
}
