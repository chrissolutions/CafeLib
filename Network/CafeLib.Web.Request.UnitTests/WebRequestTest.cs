using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.Web.Request.UnitTests
{
    public class WebRequestTest
    {
        [Fact]
        public async void GetRequestTest()
        {
            var request = new WebRequest<JToken>("https://api.github.com/users/mralexgray/repos");
            var json = await request.GetAsync();
            Assert.NotNull(json);
        }

        [Fact]
        public async void PostRequestTest()
        {
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JToken>("https://httpbin.org/anything");

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PostAsync(jsonBody);

            Assert.NotNull(json);
        }

        [Fact]
        public async void PutRequestTest()
        {
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JToken>("https://httpbin.org/anything");

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PutAsync(jsonBody);

            Assert.NotNull(json);
        }

        [Fact]
        public async void DeleteRequestTest()
        {
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JToken>("https://httpbin.org/anything");

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.DeleteAsync(jsonBody);

            Assert.NotNull(json);
        }
    }
}
