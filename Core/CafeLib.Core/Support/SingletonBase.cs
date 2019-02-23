using System.Threading.Tasks;
using CafeLib.Core.Async;
using CafeLib.Core.Extensions;

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

        #region Helpers

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
                if (_singleton == null)
                {
                    lock (Mutex)
                    {
                        // Create the singleton object.
                        _singleton = typeof(T).CreateInstance<T>();

                        // Asynchronous initalization of singleton.
                        AsyncTask.Run(_singleton.InitAsync);
                    }
                }
                return (T)_singleton;
            }
        }

        /// <summary>
        // Asynchronous initalization of singleton.
        /// </summary>
        /// <returns>task</returns>
        public virtual async Task InitAsync()
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}
