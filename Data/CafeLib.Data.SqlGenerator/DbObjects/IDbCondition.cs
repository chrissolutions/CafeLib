using System;

namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbCondition : IDbSelectable
    {
        Tuple<IDbBinary, IDbObject>[] Conditions { get; }
        IDbObject Else { get; }
    }
}