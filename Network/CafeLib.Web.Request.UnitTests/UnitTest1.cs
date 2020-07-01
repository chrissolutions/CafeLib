using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CafeLib.Web.Request.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public async void WebRequestTest()
        {
            var request = new WebRequest<JToken>("https://api.github.com/users/mralexgray/repos");
            var jtoken = await request.GetAsync();
            Debug.WriteLine(jtoken);
        }
    }
}
