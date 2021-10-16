using System.Text;

namespace CafeLib.Data.SqlGenerator.DbObjects.PostgresQlObjects
{
    public class PostgresQlLimit : DbLimit
    {
        public PostgresQlLimit(int offset, int fetch) : base(offset, fetch)
        {
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"limit {Fetch} offset {Offset}");

            return sb.ToString();
        }
    }
}