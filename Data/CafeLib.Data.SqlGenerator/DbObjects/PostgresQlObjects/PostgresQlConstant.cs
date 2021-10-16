using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.PostgresQlObjects
{
    public class PostgresQlConstant : SqlConstant
    {
        public override string ToString()
        {
            if (AsParam && !string.IsNullOrEmpty(ParamName))
            {
                return ParamName;
            }
            
            if (Val is bool b)
                return b ? "TRUE" : "FALSE";
            
            return base.ToString();
        }
    }
}