// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Authorization.Identity
{
    /// <summary>
    /// Represents a claim that is granted to all users within a role.
    /// </summary>
    [Table("AspNetRoleClaims")]
    public class IdentityRoleClaim : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}