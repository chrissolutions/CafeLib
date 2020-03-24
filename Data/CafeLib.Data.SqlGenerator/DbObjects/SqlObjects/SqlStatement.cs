namespace CafeLib.Data.SqlGenerator.DbObjects.SqlObjects
{
    public class SqlStatement : DbObject, IDbStatment
    {
        protected readonly IDbObject Script;

        public SqlStatement(IDbObject script)
        {
            Script = script;
        }

        public override string ToString()
        {
            return Script.ToString();
        }
    }
}