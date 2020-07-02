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
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JObject>("https://httpbin.org/anything");

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PostAsync(jsonBody);
            Assert.NotNull(json);

            var result = json.ToObject<BinResult>();
            Assert.Equal("POST", result.Method);
            Assert.Equal(endpoint, result.Url);
            Assert.Equal("Unique File Name", result.Json["CaseID"].Value<string>());
            Assert.Equal("StreamValue", result.Json["Content"].Value<string>());
        }

        [Fact]
        public async void PutRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JObject>(endpoint);

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PutAsync(jsonBody);
            Assert.NotNull(json);

            var result = json.ToObject<BinResult>();
            Assert.Equal("PUT", result.Method);
            Assert.Equal(endpoint, result.Url);
            Assert.Equal("Unique File Name", result.Json["CaseID"].Value<string>());
            Assert.Equal("StreamValue", result.Json["Content"].Value<string>());
        }

        [Fact]
        public async void DeleteRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new WebRequest<JObject>(endpoint);
            var jsonBody = JToken.Parse(jsonText);
            var result = await request.DeleteAsync(jsonBody);
            Assert.True(result);
        }
    }
}
