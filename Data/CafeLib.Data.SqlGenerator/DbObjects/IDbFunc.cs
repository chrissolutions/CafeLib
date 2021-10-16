using System;

namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbFunc : IDbSelectable
    {
        string Name { get; }
        IDbObject[] Parameters { get; }

        Type ReturnType { get; }
    }
}