﻿using System.Data;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.Sources
{
    public interface IConnectionOptions
    {
        ISqlCommandProvider CommandProcessor { get; }
        IDbObjectFactory DbObjectFactory { get; }
        IDbConnection GetConnection(string connectionString);
    }
}