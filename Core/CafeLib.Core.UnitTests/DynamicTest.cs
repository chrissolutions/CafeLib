using System;
using CafeLib.Core.UnitTests.DynamicModels;
using Xunit;
using Xunit.Abstractions;

namespace CafeLib.Core.UnitTests
{
    public class DynamicTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DynamicTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Summary method that demonstrates some
        /// of the basic behaviors.
        /// 
        /// More specific tests are provided below
        /// </summary>
        [Fact]
        public void ExpandoBasicTests()
        {
            // Set standard properties
            var ex = new User
            {
                Name = "Rick",
                Email = "rstrahl@whatsa.com",
                Active = true
            };

            // set dynamic properties that don't exist on type
            dynamic exd = ex;
            exd.Entered = DateTime.Now;
            exd.Company = "West Wind";
            exd.Accesses = 10;

            // set dynamic properties as dictionary
            ex["Address"] = "32 Kaiea";
            ex["Email"] = "rick@west-wind.com";
            ex["TotalOrderAmounts"] = 51233.99M;

            // iterate over all properties dynamic and native
            foreach (var prop in ex.GetProperties(true))
            {
                _testOutputHelper.WriteLine(prop.Key + " " + prop.Value);
            }

            // you can access plain properties both as explicit or dynamic
            Assert.Equal(ex.Name, exd.Name);

            // You can access dynamic properties either as dynamic or via IDictionary
            Assert.Equal(exd.Company, (string)ex["Company"]);
            Assert.Equal(exd.Address, (string) ex["Address"]);

            // You can access strong type properties via the collection as well (inefficient though)
            Assert.Equal(ex.Name, (string)ex["Name"]);

            // dynamic can access everything
            Assert.Equal(ex.Name, exd.Name); // native property
            Assert.Equal(ex["TotalOrderAmounts"], exd.TotalOrderAmounts); // dictionary property
        }
    }
}
