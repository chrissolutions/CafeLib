using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.Web.Request.UnitTests
{
    public class BasicApiRequestTest
    {
        [Fact]
        public async void BasicApiRequest_GetRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            var request = new BasicApiRequest();
            var json = await request.GetAsync(endpoint);
            Assert.NotNull(json);
            var result = JsonConvert.DeserializeObject<BinResult>(json);
            Assert.NotNull(result);
            Assert.Equal("GET", result.Method);
            Assert.Equal(endpoint, result.Url);
        }

        [Fact]
        public async void ApiRequest_PostRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new BasicApiRequest();

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PostAsync(endpoint, jsonBody);
            Assert.NotNull(json);

            var result = JsonConvert.DeserializeObject<BinResult>(json);
            Assert.NotNull(result);
            Assert.Equal("POST", result.Method);
            Assert.Equal(endpoint, result.Url);
            Assert.Equal("Unique File Name", result.Json["CaseID"]?.Value<string>());
            Assert.Equal("StreamValue", result.Json["Content"]?.Value<string>());
        }

        [Fact]
        public async void ApiRequest_PutRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new BasicApiRequest();

            var jsonBody = JToken.Parse(jsonText);

            var json = await request.PutAsync(endpoint, jsonBody);
            Assert.NotNull(json);

            var result = JsonConvert.DeserializeObject<BinResult>(json);
            Assert.NotNull(result);
            Assert.Equal("PUT", result.Method);
            Assert.Equal(endpoint, result.Url);
            Assert.Equal("Unique File Name", result.Json["CaseID"]?.Value<string>());
            Assert.Equal("StreamValue", result.Json["Content"]?.Value<string>());
        }

        [Fact]
        public async void ApiRequest_DeleteRequestTest()
        {
            const string endpoint = "https://httpbin.org/anything";
            const string jsonText = @"{ 
                    ""CaseID"": ""Unique File Name"",
                    ""Content"": ""StreamValue""
                    }";

            var request = new BasicApiRequest();
            var jsonBody = JToken.Parse(jsonText);
            var result = await request.DeleteAsync(endpoint, jsonBody);
            Assert.True(result);
        }
    }
}
