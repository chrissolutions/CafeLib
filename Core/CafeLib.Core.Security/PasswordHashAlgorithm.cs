﻿using System;

namespace CafeLib.Core.Security
{
    [Obsolete("Deprecated.  Use 'CafeLib.Authorization.Security' assembly instead.")]
	public enum PasswordHashAlgorithm
	{		
		//MD5 = 1, Not supported
		Sha1 = 2, 
		Sha256 = 3, 
		Sha384 = 4, 
		Sha512 = 5, 
	}
}
