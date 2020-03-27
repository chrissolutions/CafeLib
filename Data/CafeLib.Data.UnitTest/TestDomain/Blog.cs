using CafeLib.Core.Data;

namespace CafeLib.Data.UnitTest.TestDomain
{
    public class Blog : IEntity
    {
        public int BlogId { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public int? CommentCount { get; set; }
    }
}
