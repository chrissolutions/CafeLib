using System.Linq;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.SqlGenerator.Extensions
{
    public static class DbSelectExtensions
    {
        public static DbReference GetReturnEntityRef(this IDbSelect dbSelect)
        {
            var entityRefCol = dbSelect.Selection.SingleOrDefault(c => c is IDbRefColumn);
            return entityRefCol != null ? entityRefCol.Ref : dbSelect.From;
        }
    }
}