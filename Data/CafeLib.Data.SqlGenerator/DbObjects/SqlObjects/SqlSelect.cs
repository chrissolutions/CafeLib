using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CafeLib.Data.SqlGenerator.DbObjects.SqlObjects
{
    public class SqlSelect : SqlObject, IDbSelect
    {
        public SqlSelect()
        {
            Selection = new DbSelectableCollection(this);
        }

        public DbSelectableCollection Selection { get; }
        
        public DbReference From { get; set; }
        
        public IDbBinary Where { get; set; }
        
        public DbLimit Limit { get; set; }

        public IList<IDbJoin> Joins { get; } = new List<IDbJoin>();

        public IList<IDbSelectable> OrderBys { get; } = new List<IDbSelectable>();
        
        public DbGroupByCollection GroupBys { get; } = new DbGroupByCollection();

        public bool IsWrapingSelect { get; set; }

        public override string ToString()
        {
            return BuildOutput();
        }

        public string ToMergeKey()
        {
            return BuildOutput(false);
        }

        private string BuildOutput(bool includeSelection = true)
        {
            var sb = new StringBuilder();

            var offset = Limit?.Offset ?? 0;
            var fetch = Limit?.Fetch ?? 0;

            if (includeSelection)
            {
                var topStr = offset == 0 && fetch > 0 ? $"top({fetch}) " : "";
                sb.Append($"select {topStr}{Selection}");
            }

            if (From != null)
            {
                sb.AppendLine();
                sb.Append($"from {From}");
            }

            if (Joins.Count > 0)
            {
                sb.AppendLine();
                sb.Append($"{string.Join(Environment.NewLine, Joins)}");
            }

            if (Where != null)
            {
                sb.AppendLine();
                sb.Append($"where {Where}");
            }

            if (GroupBys.Any())
            {
                sb.AppendLine();
                sb.Append($"group by {GroupBys}");
            }

            if (OrderBys.Any())
            {
                sb.AppendLine();
                sb.Append($"order by {string.Join(", ", OrderBys)}");
            }

            if (Limit != null && Limit.Offset > 0)
            {
                sb.AppendLine();
                sb.Append($"{Limit}");
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}