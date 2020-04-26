using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string GetScript(string resourceName)
        {
            return GetScript(Assembly.GetCallingAssembly(), resourceName);
        }

        public static string GetScript<T>(this T domain, string resourceName) where T : Domain
        {
            return GetScript(domain.GetType().Assembly, resourceName);
        }

        public static string GetScript(Assembly assembly, string resourceName)
        {
            var manifest = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains(resourceName));
            return ScriptResourceDictionary.GetOrAdd(manifest, () => LoadScript(assembly, manifest));
        }

        private static string LoadScript(Assembly assembly, string manifest)
        {
            var stream = assembly.GetManifestResourceStream(manifest);
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