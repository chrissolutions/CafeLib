using CafeLib.Core.Extensions;
using CafeLib.Core.UnitTests.TypeModels;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class TypeTests
    {
        [Fact]
        public void CreateInstanceOfDefaultParameterType()
        {
            var result = GetType().CreateInstance<TypeWithDefaultConstructor>();
            Assert.NotNull(result);
            Assert.IsType<TypeWithDefaultConstructor>(result);
            Assert.Equal(100, result.Default);
        }

        [Fact]
        public void CreateInstanceOfSingleParameterType()
        {
            var result = GetType().CreateInstance<TypeWithSingleParameterConstructor>(100);
            Assert.NotNull(result);
            Assert.IsType<TypeWithSingleParameterConstructor>(result);
            Assert.Equal(100, result.Argument);
        }
    }
}
