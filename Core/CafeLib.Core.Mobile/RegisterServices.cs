using CafeLib.Core.IoC;
using CafeLib.Core.Mobile.Services;
using CafeLib.Core.Support;

namespace CafeLib.Core.Mobile
{
    public sealed class RegisterServices : SingletonBase<RegisterServices>
    {
        #region Private Variable

        private IPageService _pageService;

        #endregion

        #region Automatic Properties

        internal static IPageService PageService => Instance._pageService;

        internal static INavigationService NavigationService => (INavigationService)Instance._pageService;

        #endregion

        #region Methods

        /// <summary>
        /// Register mobile service.
        /// </summary>
        /// <param name="bootstrapper">bootstrapper used to find assembly</param>
        private void RegisterInternal(IBootstrapper bootstrapper)
        {
            if (PageService != null) return;
            _pageService = new PageService(bootstrapper);
            ServiceProvider.Register<IPageService>(p => PageService);
            ServiceProvider.Register<INavigationService>(p => NavigationService);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Register mobile services.
        /// </summary>
        /// <param name="bootstrapper">bootstrapper used to find assembly</param>
        public static void Register(IBootstrapper bootstrapper)
        {
            Instance.RegisterInternal(bootstrapper);
        }

        #endregion
    }
}
