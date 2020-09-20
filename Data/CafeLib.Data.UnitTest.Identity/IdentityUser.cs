using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Data.UnitTest.Identity
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    [Table("AspNetUsers")]
    public class IdentityUser : IEntity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
            SecurityStamp = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }

        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString() => UserName;
    }
}
