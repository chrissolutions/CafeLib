using System.Collections.Generic;
using System.Linq;
using CafeLib.Core.Collections;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;
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
            @"The Line: UPDATE dbo.Vehicles SET Vehicles.VehicleTypeID = {disable} WHERE Vehicles.ID = {vehicleId}";

        [Fact]
        public void StringRenderTest()
        {
            var rendered = RenderFormat.Render(new { disable = true, vehicleId = 3333 });
            Assert.Equal(@"The Line: UPDATE dbo.Vehicles SET Vehicles.VehicleTypeID = True WHERE Vehicles.ID = 3333", rendered);
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

        [Fact]
        public void ForEachWithIndexTest()
        {
            var list = new List<string> {"Ape", "Bunny", "Cat", "Dog", "Elephant", "Fox"};
            var testList = new List<string>();

            list.ForEach((x, i) =>
            {
                testList.Insert(i, x);
            });

            var same = !list.Except(testList).Any() && !testList.Except(list).Any();
            Assert.True(same);
        }

        [Fact]
        public void ByteCombineTest()
        {
            var array1 = new byte[] { 2, 3, 4, 5 };
            var array2 = new byte[] { 6, 7, 8, 9 };
            var array3 = new byte[] { 10, 11, 12, 13 };

            var merged = array1.Concat(array2, array3);
            Assert.True(merged.SequenceEqual(new byte[] {2,3,4,5,6,7,8,9,10,11,12,13}));
        }

        [Fact]
        public void ConverterTest()
        {
            var intVal = (int) Converter.ConvertTo(typeof(int), "5");
            Assert.Equal(5, intVal);

            intVal = Converter.Convert<int>("5");
            Assert.Equal(5, intVal);
            
            intVal = (int)Converter.ConvertTo<string>(typeof(int), "5");
            Assert.Equal(5, intVal);

            intVal = Converter.Convert<int, string>("5");
            Assert.Equal(5, intVal);

            var boolVal = Converter.Convert<bool>("0");
            Assert.False(boolVal);
        }
    }
}
