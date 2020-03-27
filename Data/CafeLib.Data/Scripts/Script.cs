using System.Collections.Generic;
using System.Text;
using CafeLib.Core.Data;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.Scripts
{
    public static class Script
    {
        #region Resources

        private static readonly IDictionary<string, string> ScriptResourceDictionary = new Dictionary<string, string>();

        public static string GetScript<T>(this T domain, string resourceName) where T : Domain
        {
            var manifest = $"{domain.GetType().Namespace}.Scripts.{resourceName}.sql";
            return ScriptResourceDictionary.GetOrAdd(manifest, () => LoadScript(domain, manifest));
        }

        private static string LoadScript<T>(T domain, string manifest) where T : Domain
        {
            var stream = domain.GetType().Assembly.GetManifestResourceStream(manifest);
            return stream.ToTextString();
        }

        #endregion

        #region Paginate

        public static string ResolvePaging(string sql, int pageNumber, int pageSize)
        {
            var result = sql;
            if (pageNumber >= 1 && pageSize > 0)
            {
                var sb = new StringBuilder(sql);
                sb.AppendLine($"OFFSET {pageSize * (pageNumber - 1)} ROWS FETCH NEXT {pageSize} ROWS ONLY");
                result = sb.ToString();
            }

            return result;
        }

        #endregion

        #region Sorting

        public static string ResolveOrdering(string sql, string columnName, int direction)
        {
            var sb = new StringBuilder(sql);
            sb.AppendLine($" ORDER BY {columnName} ASC");
            return sb.ToString();
        }

        #endregion
    }
}