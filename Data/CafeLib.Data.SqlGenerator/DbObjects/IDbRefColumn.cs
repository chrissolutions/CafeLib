namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbRefColumn : IDbSelectable
    {
        IDbRefColumn RefTo { get; set; }
    }
}