using System.IO;
using CafeLib.Data.Sources.Sqlite;
using CafeLib.Data.SqlGenerator.MethodTranslators;
using CafeLib.Data.UnitTest.Identity;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Data.UnitTest.IdentityAccess
{
    public class IdentityDatabase : IDatabase
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

        public IdentityStorage GetIdentityStorage()
        {
            return (IdentityStorage) GetStorage();
        }

        public IStorage GetStorage()
        {
            return _storage;
        }
    }
}