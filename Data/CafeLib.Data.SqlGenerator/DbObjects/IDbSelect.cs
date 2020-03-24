using System.Collections.Generic;

namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbSelect : IDbObject
    {
        // columns or expression that the query return as result columns
        DbSelectableCollection Selection { get; }
        
        DbReference From { get; }
        
        IDbBinary Where { get; set; }
        
        DbLimit Limit { get; set; }

        IList<IDbJoin> Joins { get; }

        IList<IDbSelectable> OrderBys { get; }
        
        DbGroupByCollection GroupBys { get; }

        string ToMergeKey();
    }
}