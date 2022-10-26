using System.Threading.Tasks;

namespace CafeLib.Core.Eventing.Subscribers
{
    internal interface IInvokeSubscriber<in T> where T : IEventMessage
    {
        Task Invoke(T message);
    }
}
