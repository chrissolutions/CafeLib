using CafeLib.Core.Collections;
using CafeLib.Core.Extensions;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class CoreTests
    {
        //private const string ParamFormat =
        //    @"UPDATE dbo.Vehicles
        //        SET Vehicles.VehicleTypeID = {disable}
        //        WHERE Vehicles.ID = {vehicleId}";

        private const string RenderFormat =
            @"UPDATE dbo.Vehicles SET Vehicles.VehicleTypeID = {disable} WHERE Vehicles.ID = {vehicleId}";

        [Fact]
        public void StringRenderTest()
        {
            var rendered = RenderFormat.Render(new { disable = true, vehicleId = 3333 });
            Assert.Equal(@"UPDATE dbo.Vehicles SET Vehicles.VehicleTypeID = True WHERE Vehicles.ID = 3333", rendered);
        }

        [Fact]
        public void ThreadSafeDictionaryTest()
        {
            var dictionary = new ThreadSafeDictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            Assert.Equal("value1", dictionary["key1"]);
            Assert.Equal("value2", dictionary["key2"]);

            dictionary.Add("key3", "value3");
            Assert.Equal("value3", dictionary["key3"]);

            var index = 0;
            foreach (var key in dictionary.Keys)
            {
                Assert.Equal($"key{++index}", key);
            }

            dictionary["key2"] = "newValue2";
            Assert.Equal("newValue2", dictionary["key2"]);
        }
    }
}
