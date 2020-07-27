using System;
using System.Collections.Generic;
using CafeLib.Core.UnitTests.DynamicModels;
using Newtonsoft.Json;
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

        [Fact]
        public void AddAndReadDynamicPropertiesTest()
        {
            // strong typing first
            var ex = new User {Name = "Rick", Email = "rick@whatsa.com"};

            // create dynamic and create new props
            dynamic exd = ex;

            string company = "West Wind";
            int count = 10;

            exd.entered = DateTime.Now;
            exd.Company = company;
            exd.Accesses = count;

            Assert.Equal(exd.Company, company);
            Assert.Equal(exd.Accesses, count);
        }

        [Fact]
        public void AddAndReadDynamicIndexersTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            string address = "32 Kaiea";

            ex["Address"] = address;
            ex["Contacted"] = true;

            dynamic exd = ex;

            Assert.Equal(exd.Address, ex["Address"]);
            Assert.Equal(exd.Contacted, true);
        }

        [Fact]
        public void PropertyAsIndexerTest()
        {
            // Set standard properties
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            Assert.Equal(ex.Name, ex["Name"]);
            Assert.Equal(ex.Entered, ex["Entered"]);
        }

        [Fact]
        public void DynamicAccessToPropertyTest()
        {
            // Set standard properties
            var ex = new ExpandoInstance {Name = "Rick", Entered = DateTime.Now};

            // turn into dynamic
            dynamic exd = ex;

            // Dynamic can access 
            Assert.Equal(ex.Name, exd.Name);
            Assert.Equal(ex.Entered, exd.Entered);
        }

        [Fact]
        public void IterateOverDynamicPropertiesTest()
        {
            var ex = new ExpandoInstance();
            ex.Name = "Rick";
            ex.Entered = DateTime.Now;

            dynamic exd = ex;
            exd.Company = "West Wind";
            exd.Accesses = 10;

            // Dictionary pseudo implementation
            ex["Count"] = 10;
            ex["Type"] = "NEWAPP";

            // Dictionary Count - 2 dynamic props added
            Assert.True(ex.Properties.Count == 4);

            // iterate over all properties
            foreach (KeyValuePair<string, object> prop in exd.GetProperties(true))
            {
                _testOutputHelper.WriteLine(prop.Key + " " + prop.Value);
            }
        }

        [Fact]
        public void MixInObjectInstanceTest()
        {
            // Create expando an mix-in second objectInstanceTest
            var ex = new ExpandoInstance(new Address()) {Name = "Rick", Entered = DateTime.Now};

            // create dynamic
            dynamic exd = ex;

            // values should show Addresses initialized values (not null)
            Console.WriteLine(exd.FullAddress);
            Console.WriteLine(exd.Email);
            Console.WriteLine(exd.Phone);
        }

        [Fact]
        public void TwoWayJsonSerializeExpandoTyped()
        {
            // Set standard properties
            var ex = new User()
            {
                Name = "Rick",
                Email = "rstrahl@whatsa.com",
                Password = "Seekrit23",
                Active = true
            };

            // set dynamic properties
            dynamic exd = ex;
            exd.Entered = DateTime.Now;
            exd.Company = "West Wind";
            exd.Accesses = 10;

            // set dynamic properties as dictionary
            ex["Address"] = "32 Kaiea";
            ex["Email"] = "rick@west-wind.com";
            ex["TotalOrderAmounts"] = 51233.99M;

            // *** Should serialize both static properties dynamic properties
            var json = JsonConvert.SerializeObject(ex, Formatting.Indented);
            _testOutputHelper.WriteLine("*** Serialized Native object:");
            _testOutputHelper.WriteLine(json);

            Assert.Contains("Name", json); // static
            Assert.Contains("Company", json); // dynamic


            // *** Now deserialize the JSON back into object to 
            // *** check for two-way serialization
            var user2 = JsonConvert.DeserializeObject<User>(json);
            json = JsonConvert.SerializeObject(user2, Formatting.Indented);
            _testOutputHelper.WriteLine("*** De-Serialized User object:");
            _testOutputHelper.WriteLine(json);

            Assert.Contains("Name", json); // static
            Assert.Contains("Company", json); // dynamic
        }
    }
}
