namespace CafeLib.Data.SqlGenerator.DbObjects.SqlObjects
{
    public class SqlKeyWord : SqlObject, IDbKeyWord
    {
        public string KeyWord { get; set; }

        public override string ToString()
        {
            return $"{KeyWord}";
        }
    }
}