using System.Threading.Tasks;

namespace CafeLib.Core.Queueing
{
    public interface IQueueConsumer<in T>
    {
        Task Consume(T item);
    }
}
