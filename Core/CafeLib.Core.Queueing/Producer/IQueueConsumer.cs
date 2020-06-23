using System.Threading.Tasks;

namespace CafeLib.Core.Queueing.Producer
{
    public interface IQueueConsumer<in T>
    {
        Task Consume(T item);
    }
}
