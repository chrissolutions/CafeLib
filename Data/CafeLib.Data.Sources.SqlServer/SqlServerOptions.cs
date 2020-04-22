﻿using System.Data;
using System.Data.SqlClient;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
using RepoDb;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Sources.SqlServer
{
    public class SqlServerOptions : IConnectionOptions
    {
        public ISqlCommandProvider CommandProvider { get; } = SqlServerCommandProvider.Current;
        public IDbObjectFactory DbObjectFactory { get; } = new SqlObjectFactory();
        public IDbConnection GetConnection(string connectionString) => new SqlConnection(connectionString);
    }
}
