namespace CafeLib.Data.SqlGenerator.DbObjects
{
    public interface IDbSelectable : IDbObject
    {
        DbReference Ref { get; set; }

        IDbSelect OwnerSelect { get; set; }

        string Alias { get; set; }

        bool IsJoinKey { get; set; }

        bool IsAggregation { get; set; }

        string ToSelectionString();
    }
}