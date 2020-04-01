using System;

namespace CafeLib.Data.SqlGenerator.DbObjects.SqlObjects
{
    public class SqlSelectable : SqlObject, IDbSelectable 
    {
        public DbReference Ref { get; set; }

        public IDbSelect OwnerSelect { get; set; }

        public string Alias { get; set; }

        public bool IsJoinKey { get; set; }

        public bool IsAggregation { get; set; }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public virtual string ToSelectionString()
        {
            return string.IsNullOrEmpty(Alias) ? $"{this}" : $"{this} as {QuotationMark}{Alias}{QuotationMark}";
        }

        protected bool Equals(SqlSelectable other)
        {
            return string.Equals(Alias, other.Alias) &&
                   Equals(OwnerSelect, other.OwnerSelect) &&
                   Equals(Ref, other.Ref) &&
                   IsJoinKey == other.IsJoinKey;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SqlSelectable other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Alias?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (OwnerSelect?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Ref?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ IsJoinKey.GetHashCode();
                return hashCode;
            }
        }
    }
}