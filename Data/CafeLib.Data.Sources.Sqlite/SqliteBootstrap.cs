using System.Data.SQLite;
using CafeLib.Core.Support;
using RepoDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;

namespace CafeLib.Data.Sources.Sqlite
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="SQLiteConnection"/> object.
    /// </summary>
    public class SqliteBootstrap : SingletonBase<SqliteBootstrap>
    {
        private bool _isInitialized;
        private static readonly object Mutex = new object();

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for SqLite.
        /// </summary>
        public static void Initialize(IDbSetting setting = null)
        {
            Instance.Setup(setting);
        }

        #endregion

        #region Helpers

        private void Setup(IDbSetting dbSetting)
        {
            if (_isInitialized) return;

            lock (Mutex)
            {
                DbSettingMapper.Add(typeof(SQLiteConnection), dbSetting ?? new SqLiteDbSetting(), true);

                // Map the DbHelper
                DbHelperMapper.Add(typeof(SQLiteConnection), new SqLiteDbHelper(), true);

                // Map the Statement Builder
                StatementBuilderMapper.Add(typeof(SQLiteConnection), new SqLiteStatementBuilder(), true);

                // Set the flag
                _isInitialized = true;
            }
        }

        #endregion 
    }
}