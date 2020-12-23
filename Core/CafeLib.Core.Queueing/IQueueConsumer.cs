using System.Threading.Tasks;

namespace CafeLib.Core.Queueing
{
    public interface IQueueConsumer
    {
        Task Consume(object item);
    }

    public interface IQueueConsumer<in T> : IQueueConsumer
    {
        Task Consume(T item);
    }
}
