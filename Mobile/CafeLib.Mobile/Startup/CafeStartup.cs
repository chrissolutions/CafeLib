using System;
using System.Diagnostics.CodeAnalysis;
using CafeLib.Core.Support;

namespace CafeLib.Mobile.Startup
{
    /// <summary>
    /// Base class of CafeLib mobile application.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class CafeStartup<T> where T : CafeApplication
    {
        private readonly ICafeStartup _cafeStartup;
        private readonly CafeRegistry _cafeRegistry;
        private readonly T _application;

        public CafeStartup(ICafeStartup cafeStartup)
        {
            _cafeStartup = cafeStartup ?? throw new ArgumentNullException(nameof(cafeStartup));
            _cafeRegistry = new CafeRegistry();
            _application = Creator.CreateInstance<T>(_cafeRegistry);
        }

        public T Configure()
        {
            _cafeStartup.Configure(_cafeRegistry);
            _application.Configure();
            return _application;
        }
    }
}
