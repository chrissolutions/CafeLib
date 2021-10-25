using System.IO;
using CafeLib.Data.Sources.Sqlite;
using CafeLib.Data.UnitTest.Identity;

namespace CafeLib.Data.UnitTest.IdentityAccess
{
    public class IdentityDatabase : IDatabase<IdentityStorage>
    {
        private readonly IdentityStorage _storage;

        public string DatabaseName { get; }
        public string ConnectionString { get; }

        internal string DatabaseFilePath { get; }

        /// <summary>
        /// Identity database constructor.
        /// </summary>
        public IdentityDatabase()
        {
            ConnectionString = "Data Source=identity.db";
            DatabaseFilePath = ConnectionString.Split(new[] { '=' })[1];
            DatabaseName = DatabaseFilePath;
            _storage = new IdentityStorage(ConnectionString, new SqliteOptions());

            if (!File.Exists(DatabaseFilePath))
            {
                _storage.CreateDatabase();
                var seeder = new IdentitySeeder();
                seeder.UsersSeed(_storage);
            }
        }

        public IdentityStorage GetStorage()
        {
            return _storage;
        }
    }
}