using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CafeLib.Data.Sources
{
    internal class DbNullConnection : DbConnection
    {
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override string ConnectionString { get; set; }
        public override string Database { get; } = null;
        public override ConnectionState State { get; } = ConnectionState.Broken;
        public override string DataSource { get; } = null;
        public override string ServerVersion { get; } = null;

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }
    }
}
