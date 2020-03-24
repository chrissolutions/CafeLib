namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbOrderByColumn : IDbSelectable
    {
        IDbSelectable DbSelectable { get; set; }
        DbOrderDirection Direction { get; }
    }
}