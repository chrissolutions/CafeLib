using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
using CafeLib.Core.UnitTests.TypeModels;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class TypeTests
    {
        [Fact]
        public void CreateInstanceOfDefaultParameterType()
        {
            var result = Creator.CreateInstance<TypeWithDefaultConstructor>();
            Assert.NotNull(result);
            Assert.IsType<TypeWithDefaultConstructor>(result);
            Assert.Equal(100, result.Default);
        }

        [Fact]
        public void CreateInstanceWithSingleParameter()
        {
            var result = Creator.CreateInstance<TypeWithParametersConstructor>(100);
            Assert.NotNull(result);
            Assert.IsType<TypeWithParametersConstructor>(result);
            Assert.Equal(100, result.Argument1);
        }

        [Fact]
        public void CreateInstanceWithTwoParameters()
        {
            var result = Creator.CreateInstance<TypeWithParametersConstructor>(100, 200);
            Assert.NotNull(result);
            Assert.IsType<TypeWithParametersConstructor>(result);
            Assert.Equal(100, result.Argument1);
            Assert.Equal(200, result.Argument2);
        }

        [Fact]
        public void CreateInstanceFromType()
        {
            var instance = Creator.CreateInstance(typeof(TypeWithDefaultConstructor));
            Assert.NotNull(instance);
            Assert.IsType<TypeWithDefaultConstructor>(instance);
            var result = (TypeWithDefaultConstructor)instance;
            Assert.Equal(100, result.Default);
        }
    }
}
