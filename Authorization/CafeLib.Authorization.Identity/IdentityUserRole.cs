// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Authorization.Identity
{
    /// <summary>
    /// Represents the link between a user and a role.
    /// </summary>
    [Table("AspNetUserRoles")]
    public class IdentityUserRole : IEntity
    {
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        public string RoleId { get; set; }
    }
}