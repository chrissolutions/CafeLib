// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Authorization.Identity
{
    /// <summary>
    /// Represents a login and its associated provider for a user.
    /// </summary>
    [Table("AspNetUserLogins")]
    public class IdentityUserLogin : IEntity
    {
        [Key]
        [Column(Order = 1)]
        public string LoginProvider { get; set; }
        [Key]
        [Column(Order = 2)]
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }
    }
}