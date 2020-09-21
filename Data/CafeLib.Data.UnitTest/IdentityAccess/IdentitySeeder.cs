using System.Collections.Generic;
using CafeLib.Authorization.Hash;
using CafeLib.Data.Extensions;
using CafeLib.Data.UnitTest.Identity;

namespace CafeLib.Data.UnitTest.IdentityAccess
{
    public class IdentitySeeder
    {
        public void UsersSeed(IdentityStorage storage)
        {
            const string password = "My long 123$ password";

            var alice = new IdentityUserDto
            {
                Id = "1",
                UserName = "alice",
                NormalizedUserName = "ALICE",
                Email = "AliceSmith@email.com",
                NormalizedEmail = "AliceSmith@email.com".ToUpper(),
                EmailConfirmed = 1,
                PasswordHash = PasswordHash.Default.HashPassword(password)
            };

            var bob = new IdentityUserDto
            {
                Id = "2",
                UserName = "bob",
                NormalizedUserName = "BOB",
                Email = "BobSmith@email.com",
                NormalizedEmail = "bobsmith@email.com".ToUpper(),
                EmailConfirmed = 1,
                PasswordHash = PasswordHash.Default.HashPassword(password)
            };

            storage.Save(alice);
            storage.Save(bob);

            var claims = new List<IdentityUserClaim>
            {
                new IdentityUserClaim
                {
                    Id = 1,
                    UserId = "1",
                    ClaimType = "name",
                    ClaimValue = "Alice Smith"
                },
                new IdentityUserClaim
                {
                    Id = 2,
                    UserId = "1",
                    ClaimType = "given_name",
                    ClaimValue = "Alice"
                },
                new IdentityUserClaim
                {
                    Id = 3,
                    UserId = "1",
                    ClaimType = "family_name",
                    ClaimValue = "Smith"
                },
                new IdentityUserClaim
                {
                    Id = 4,
                    UserId = "1",
                    ClaimType = "email",
                    ClaimValue = "AliceSmith@email.com"
                },
                new IdentityUserClaim
                {
                    Id = 5,
                    UserId = "1",
                    ClaimType = "website",
                    ClaimValue = "http://alice.com"
                },
                new IdentityUserClaim
                {
                    Id = 6,
                    UserId = "2",
                    ClaimType = "name",
                    ClaimValue = "Bob Smith"
                },
                new IdentityUserClaim
                {
                    Id = 7,
                    UserId = "2",
                    ClaimType = "given_name",
                    ClaimValue = "Bob"
                },
                new IdentityUserClaim
                {
                    Id = 8,
                    UserId = "2",
                    ClaimType = "family_name",
                    ClaimValue = "Smith"
                },
                new IdentityUserClaim
                {
                    Id = 9,
                    UserId = "2",
                    ClaimType = "email",
                    ClaimValue = "BobSmith@email.com"
                },
                new IdentityUserClaim
                {
                    Id = 10,
                    UserId = "2",
                    ClaimType = "website",
                    ClaimValue = "http://bob.com"
                },
                new IdentityUserClaim
                {
                    Id = 11,
                    UserId = "1",
                    ClaimType = "email_verified",
                    ClaimValue = true.ToString()
                },
                new IdentityUserClaim
                {
                    Id = 12,
                    UserId = "2",
                    ClaimType = "email_verified",
                    ClaimValue = true.ToString()
                },
                new IdentityUserClaim
                {
                    Id = 13,
                    UserId = "1",
                    ClaimType = "address",
                    ClaimValue =
                        @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }"
                },
                new IdentityUserClaim
                {
                    Id = 14,
                    UserId = "2",
                    ClaimType = "address",
                    ClaimValue =
                        @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }"
                },
                new IdentityUserClaim
                {
                    Id = 15,
                    UserId = "1",
                    ClaimType = "location",
                    ClaimValue = "somewhere"
                }
            };

            storage.Save(claims);
        }
    }
}
