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
    }
}
