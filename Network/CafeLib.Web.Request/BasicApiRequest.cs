using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CafeLib.Web.Request
{
    public class BasicApiRequest : ApiRequest<string, JToken>
    {
    }
}
