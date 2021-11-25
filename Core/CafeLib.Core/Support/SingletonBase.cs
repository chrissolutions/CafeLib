using System.Threading.Tasks;

namespace CafeLib.Core.Support
{
    /// <summary>
    /// Singleton base class wrapper
    /// </summary>
    /// <typeparam name="T">singleton type</typeparam>
    public abstract class SingletonBase<T> where T : SingletonBase<T>
    {
        #region Private Variables

        private static SingletonBase<T> _singleton;
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Mutex = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Create singleton instance.
        /// </summary>
        protected SingletonBase()
        {
            // Set singleton.
            _singleton = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Obtain the current instance.
        /// </summary>
        public static T Current => Instance;

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronous initialization of singleton.
        /// </summary>
        /// <returns>task</returns>
        public virtual async Task InitAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        #endregion 

        #region Protected Methods

        /// <summary>
        /// Releases the singleton.
        /// </summary>
        protected void ReleaseSingleton()
        {
            _singleton = null;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        protected static T Instance
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_singleton == null)
                {
                    lock (Mutex)
                    {
                        // Create the singleton object.
                        _singleton = Creator.CreateInstance<T>();

                        // Asynchronous initialization of singleton.
                        Task.Run(async () => await _singleton.InitAsync().ConfigureAwait(false));
                    }
                }

                return (T)_singleton;
            }
        }

        #endregion
    }
}
