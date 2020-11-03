using System.Data.SQLite;
using CafeLib.Core.Support;
using RepoDb;
using RepoDb.DbHelpers;
using RepoDb.DbSettings;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using RepoDb.StatementBuilders;

namespace CafeLib.Data.Sources.Sqlite
{
    /// <summary>
    /// A class used to initialize necessary objects that is connected to <see cref="SQLiteConnection"/> object.
    /// </summary>
    public class SqliteBootstrap : SingletonBase<SqliteBootstrap>
    {
        private bool _isInitialized;
        private static readonly object _mutex = new object();

        #region Methods

        /// <summary>
        /// Initializes all necessary settings for SqLite.
        /// </summary>
        public static void Initialize(IDbSetting setting = null)
        {
            Instance.Setup(setting ?? new SqLiteDbSetting());
        }

        #endregion

        #region Helpers

        private void Setup(IDbSetting settings)
        {
            if (_isInitialized) return;

            lock (_mutex)
            {
                DbSettingMapper.Add(typeof(SQLiteConnection), settings, true);

                // Map the DbHelper
                DbHelperMapper.Add(typeof(SQLiteConnection), new SqLiteDbHelper(settings, new SdsSqLiteDbTypeNameToClientTypeResolver()), true);

                // Map the Statement Builder
                StatementBuilderMapper.Add(typeof(SQLiteConnection), new SqLiteStatementBuilder(settings), true);

                // Set the flag
                _isInitialized = true;
            }
        }

        #endregion 
    }
}