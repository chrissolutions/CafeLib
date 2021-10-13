using System;
using CafeLib.Core.MethodBinding;

namespace SignalRChannelTest
{
    public sealed class ClientBridge : MethodBridge
    {
        [MethodExport("doClientStuff")]
        public void DoClientStuff(int arg1, string arg2)
        {
            Console.WriteLine($"doClientStuff: {arg1}: {arg2}");
        }

        [MethodExport("notifyMessage")]
        public void NotifyMessage(string data)
        {
            Console.WriteLine($"Incoming data: {data}");
        }

        [MethodExport("doGuidTest")]
        public void DoGuidTest(Guid arg1, string arg2)
        {
            Console.WriteLine($"doGuidTest: {arg1}: {arg2}");
        }

        /// <summary>
        /// Override invoke to report argument exceptions
        /// </summary>
        /// <param name="method">export method name</param>
        /// <param name="args">arguments</param>
        public override void Invoke(string method, object[] args)
        {
            try
            {
                base.Invoke(method, args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Calling export {method}:{Environment.NewLine}{ex}");
            }
        }
    }
}
