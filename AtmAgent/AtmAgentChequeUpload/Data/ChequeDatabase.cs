using System.Data.SQLite;
using System.IO;
using AtmAgent.Cheques;
using AtmAgentChequeUpload.Config;
using AtmAgentChequeUpload.Files;
using AtmAgentChequeUpload.Logging;
using CafeLib.Data;
using CafeLib.Data.Scripts;
using CafeLib.Data.Sources.Sqlite;
using Dapper;

namespace AtmAgentChequeUpload.Data
{
    public class ChequeDatabase : IDatabase
    {
        public string DatabaseName { get; }
        public string ConnectionString { get; }
        internal string DatabaseFilePath { get; }

        /// <summary>
        /// Cheque dto repository constructor.
        /// </summary>
        /// <param name="config">configuration service</param>
        /// <param name="fileManager">file manager</param>
        /// <param name="logger">logging service</param>
        public ChequeDatabase(IAppConfig config, IChequeFileManager fileManager, ILogger logger)
        {
            DatabaseName = config.DatabaseFileName;
            DatabaseFilePath = Path.Combine(fileManager.GetDatabaseFolder(), DatabaseName);
            ConnectionString = $"Data Source={DatabaseFilePath}";

            if (!File.Exists(DatabaseFilePath))
            {
                CreateDatabase();
                logger.Debug($"Database {DatabaseFilePath} created.");
            }
        }

        public IStorage GetStorage()
        {
            return new ChequeStorage(ConnectionString, new SqliteOptions());
        }

        #region Helpers

        /// <summary>
        /// Create the database to save on file.
        /// </summary>
        private void CreateDatabase()
        {
            var createTable = Script.GetScript("AtmChequeTable.sql");
            var createIndex = Script.GetScript("AtmChequeIndex.sql");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            connection.Execute(createTable);
            connection.Execute(createIndex);
        }

        #endregion


    }
}
