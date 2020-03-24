namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public class DbObject : IDbObject
    {
        public DbOutputOption OutputOption { get; set; } = new DbOutputOption();

        public string QuotationMark => OutputOption.QuotationMark ?? "'";
    }
}