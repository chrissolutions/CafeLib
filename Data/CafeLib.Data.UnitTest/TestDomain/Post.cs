using CafeLib.Core.Data;

namespace CafeLib.Data.UnitTest.TestDomain
{
    public class Post : IEntity
    {
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int LikeCount { get; set; }

        public int BlogId { get; set; }

        public int UserId { get; set; }
    }
}
