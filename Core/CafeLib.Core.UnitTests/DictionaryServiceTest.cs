using System;
using CafeLib.Core.Collections;
using CafeLib.Core.IoC;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class DictionaryServiceTest
    {
        private readonly Guid _propertyGuid = Guid.NewGuid();
        protected IServiceResolver Resolver;

        public class TestObject
        {
            public int Value { get; set; }
        }

        public DictionaryServiceTest()
        {
            Resolver = IocFactory.CreateRegistry()
                .AddSingleton<IDictionaryService>(x => DictionaryService.Current)
                .GetResolver();
        }

        [Fact]
        public void DictionaryServiceSetPropertyGuidTest()
        {
            var dictionaryService = Resolver.Resolve<IDictionaryService>();
            dictionaryService.SetEntry(_propertyGuid, 20);
            Assert.Equal(20, dictionaryService.GetEntry<int>(_propertyGuid));
        }

        [Fact]
        public void DictionaryServiceSetPropertyStringTest()
        {
            var dictionaryService = Resolver.Resolve<IDictionaryService>();
            dictionaryService.SetEntry("key", 20);
            Assert.Equal(20, dictionaryService.GetEntry<int>("key"));
        }

        [Fact]
        public void DictionaryServiceSetPropertyTypeTest()
        {
            var dictionaryService = Resolver.Resolve<IDictionaryService>();
            dictionaryService.SetEntry(new TestObject { Value = 20 });
            var testObject = dictionaryService.GetEntry<TestObject>();
            Assert.Equal(20, testObject.Value);
        }

        [Fact]
        public void DictionaryServiceContainsPropertyTest()
        {
            var dictionaryService = Resolver.Resolve<IDictionaryService>();
            dictionaryService.SetEntry("name", "Kilroy");
            Assert.Equal("Kilroy", dictionaryService.GetEntry<string>("name"));

            Assert.True(dictionaryService.HasEntry("name"));

            Assert.True(dictionaryService.RemoveEntry("name"));

            Assert.False(dictionaryService.HasEntry("name"));
        }
    }
}