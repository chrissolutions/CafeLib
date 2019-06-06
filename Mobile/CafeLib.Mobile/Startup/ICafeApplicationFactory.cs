namespace CafeLib.Mobile.Startup
{
    /// <summary>
    /// Base class of CafeLib mobile application.
    /// </summary>
    public interface ICafeApplicationFactory<out T> where T : CafeApplication
    {
        T CreateApplication();
    }
}
