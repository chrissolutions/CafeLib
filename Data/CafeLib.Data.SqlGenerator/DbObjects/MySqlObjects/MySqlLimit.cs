using System.Text;

namespace CafeLib.Data.SqlGenerator.DbObjects.MySqlObjects
{
    public class MySqlLimit : DbLimit
    {
        public MySqlLimit(int offset, int fetch) : base(offset, fetch)
        {
        }
            
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"limit {Offset}, {Fetch}");
    
            return sb.ToString();
        }
    }
}