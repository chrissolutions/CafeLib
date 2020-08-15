CREATE TABLE IF NOT EXISTS 'AspNetRoles' ( 
    Id varchar(450) NOT NULL,
    Name varchar(256) NOT NULL,
    NormalizedName varchar(256) NOT NULL,
    ConcurrencyStamp longtext,
    PRIMARY KEY (Id)
);

CREATE TABLE IF NOT EXISTS 'AspNetRoleClaims' ( 
  Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  RoleId varchar(450) NOT NULL,
  ClaimType varchar(256) NULL,
  ClaimValue varchar(256) NULL,
  FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS 'AspNetUsers' (
    Id varchar (128) NOT NULL,
    UserName varchar (256) NOT NULL,
    NormalizedUserName varchar (256) NOT NULL,
    Email varchar (256) DEFAULT NULL,
    NormalizedEmail varchar (256) DEFAULT NULL,
    EmailConfirmed tinyint (1) NOT NULL,
    PasswordHash longtext,
    SecurityStamp longtext,
    ConcurrencyStamp longtext,
    PhoneNumber longtext,
    PhoneNumberConfirmed tinyint (1) NOT NULL,
    TwoFactorEnabled tinyint (1) NOT NULL,
    LockoutEnd datetime DEFAULT NULL,
    LockoutEnabled tinyint (1) NOT NULL,
    AccessFailedCount int (11) NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE IF NOT EXISTS 'AspNetUserClaims' ( 
  Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  UserId varchar(128) NOT NULL,
  ClaimType varchar(256) NULL,
  ClaimValue varchar(256) NULL,
  FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS 'AspNetUserLogins' ( 
  UserId varchar(128) NOT NULL,
  LoginProvider varchar(128) NOT NULL,
  ProviderKey varchar(128) NOT NULL,
  PRIMARY KEY (UserId, LoginProvider, ProviderKey),
  FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS 'AspNetUserRoles' ( 
  UserId varchar(128) NOT NULL,
  RoleId varchar(128) NOT NULL,
  PRIMARY KEY (UserId, RoleId),
  FOREIGN KEY (RoleId) REFERENCES AspNetRoles (Id) ON DELETE CASCADE,
  FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims (UserId);

CREATE INDEX IX_AspNetUserLogins_UserId ON AspNetUserLogins (UserId);

CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles (RoleId);

CREATE INDEX IX_AspNetUserRoles_UserId ON AspNetUserRoles (UserId);
