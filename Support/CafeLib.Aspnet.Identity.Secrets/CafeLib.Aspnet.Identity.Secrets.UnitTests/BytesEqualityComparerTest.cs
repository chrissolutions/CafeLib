using Xunit;
// ReSharper disable ExpressionIsAlwaysNull

namespace CafeLib.Aspnet.Identity.Secrets.UnitTests
{
    public class BytesEqualityComparerTest
    {
        private readonly BytesEqualityComparer _comparer;

        public BytesEqualityComparerTest()
        {
            _comparer = new BytesEqualityComparer();
        }

        [Fact]
        public void References_Null()
        {
            // Arrange
            byte[] x = null;
            byte[] y = null;

            // Act & assert - success case
            Assert.True(_comparer.Equals(x, y));
        }

        [Fact]
        public void References_Null_Valid_Array()
        {
            // Arrange
            byte[] x = null;
            var y = new byte[] { 1, 2, 3 };

            // Act & assert - success case
            Assert.False(_comparer.Equals(x, y));
        }

        [Fact]
        public void References_Same()
        {
            // Arrange
            var x = new byte[] { 1, 2, 3 };
            var y = x;

            // Act & assert - success case
            Assert.True(_comparer.Equals(x, y));

            // Arrange
            x = y;

            // Act & assert - success case
            Assert.True(_comparer.Equals(x, y));
        }

        [Fact]
        public void Values_Same()
        {
            // Arrange
            var x = new byte[] { 1, 2, 3 };
            var y = new byte[] { 1, 2, 3 };

            // Act & assert - success case
            Assert.True(_comparer.Equals(x, y));
        }

        [Fact]
        public void Values_Different()
        {
            // Arrange
            var x = new byte[] { 1, 2, 3 };
            var y = new byte[] { 1, 2 };

            // Act & assert - failure case
            Assert.False(_comparer.Equals(x, y));
        }

    }
}