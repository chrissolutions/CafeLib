using System.Collections.Generic;

namespace CafeLib.Core.Logging
{
    public class LogMessageInfo : Dictionary<string, object>
    {
        internal LogMessageInfo(IDictionary<string, object> items)
            : base(items)
        {
        }
    }
}
