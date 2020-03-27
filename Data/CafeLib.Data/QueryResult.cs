using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Data;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data
{
    /// <summary>
    /// QueryResult structure.
    /// </summary>
    public struct QueryResult<T> where T : class, IDataModel
    {
        public int TotalCount { get; set; }

        public IEnumerable<T> Records { get; set; }

        public QueryResult(IQueryable<T> records, int totalCount = -1)
            : this(records.ToArray(), totalCount)
        {
        }

        public QueryResult(IEnumerable<T> records, int totalCount = -1)
        {
            Records = records;
            TotalCount = totalCount == -1 ? Records.Count() : totalCount;
        }
    }
}