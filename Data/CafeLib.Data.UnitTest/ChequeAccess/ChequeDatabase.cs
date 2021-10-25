using CafeLib.Data.Sources.Sqlite;

namespace CafeLib.Data.UnitTest.ChequeAccess
{
    public class ChequeDatabase : IDatabase<ChequeStorage>
    {
        private readonly ChequeStorage _storage;

        public string DatabaseName { get; }
        public string ConnectionString { get; }

        internal string DatabaseFilePath { get; }

        /// <summary>
        /// Identity database constructor.
        /// </summary>
        public ChequeDatabase()
        {
            ConnectionString = "Data Source=cheques.sqlite";
            DatabaseFilePath = ConnectionString.Split(new[] { '=' })[1];
            DatabaseName = DatabaseFilePath;
            _storage = new ChequeStorage(ConnectionString, new SqliteOptions());
        }

        public ChequeStorage GetStorage()
        {
            return _storage;
        }
    }
}