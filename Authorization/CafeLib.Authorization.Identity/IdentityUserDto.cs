using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Authorization.Identity
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    [Table("AspNetUsers")]
    public class IdentityUserDto : IEntity
    {
        /// <summary>
        /// IdentityUserDto constructor./>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public IdentityUserDto()
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
        public IdentityUserDto(string userName)
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
        public byte EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public byte PhoneNumberConfirmed { get; set; }
        public byte TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public byte LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString() => UserName;
    }
}
