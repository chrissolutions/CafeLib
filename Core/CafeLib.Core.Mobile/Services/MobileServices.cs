using CafeLib.Core.IoC;
using CafeLib.Core.Support;

namespace CafeLib.Core.Mobile.Services
{
    public sealed class MobileServices : SingletonBase<MobileServices>
    {
        #region Private Variable

        private readonly IPageService _pageService;

        #endregion

        /// <summary>
        /// ServiceProvider instance constructor.
        /// </summary>
        private MobileServices()
        {
            _pageService = new MobileService();
            ServiceProvider.Register<IPageService>(p => PageService);
            ServiceProvider.Register<INavigationService>(p => NavigationService);
            ServiceProvider.Register<IDeviceService>(p => DeviceService);
        }

        #region Automatic Properties

        internal static IPageService PageService => Instance._pageService;

        internal static INavigationService NavigationService => (INavigationService)Instance._pageService;

        internal static IDeviceService DeviceService => (IDeviceService)Instance._pageService;

        #endregion

        #region Static Methods

        /// <summary>
        /// Register mobile services.
        /// </summary>
        public static void Register()
        {
            Instance.InitAsync().Wait();
        }

        #endregion
    }
}
