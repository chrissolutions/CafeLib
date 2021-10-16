using System.Collections.Generic;

namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbList<T> : IDbObject, IList<T> where T : IDbObject
    {
        IList<T> Items { get; } 
    }
}