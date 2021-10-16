// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using CafeLib.Core.Data;

namespace CafeLib.Authorization.Identity
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    [Table("AspNetUserTokens")]
    public class IdentityUserToken : IEntity
    {
        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public string Value { get; set; }
    }
}