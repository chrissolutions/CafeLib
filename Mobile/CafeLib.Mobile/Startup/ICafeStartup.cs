using CafeLib.Core.IoC;

namespace CafeLib.Mobile.Startup
{
    /// <summary>
    /// Cafe startup interface.
    /// </summary>
    public interface ICafeStartup
    {
        void Configure(IServiceRegistry registry);
    }
}
