using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Mobile.Commands
{
    public interface IXamAsyncCommand : IXamCommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public interface IXamAsyncCommand<in T> : IXamCommand<T>
    {
        Task ExecuteAsync(T parameter);

        bool CanExecute(T parameter);
    }

    public interface IXamAsyncCommand<in TParameter, TResult> : IXamCommand<TParameter>
    {
        Task<TResult> ExecuteAsync(TParameter parameter);
    }
}
