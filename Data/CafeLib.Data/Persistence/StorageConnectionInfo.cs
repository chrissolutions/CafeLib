using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using CafeLib.Core.Extensions;
using CafeLib.Data.Options;
using CafeLib.Data.Options.SqlServer;

namespace CafeLib.Data.Persistence
{
    internal class StorageConnectionInfo : IConnectionInfo
    {
        private const string ConnectionUriKey = "CONNECTIONURI";
        private const string ConnectionSchemeKey = "CONNECTIONSCHEME";
        private const string ConnectionNameKey = "CONNECTIONNAME";
        private const string ConnectionStringKey = "CONNECTIONSTRINGKEY";
        private const string HostNameKey = "HOSTNAME";
        private const string PortKey = "PORT";
        private const string UserNameKey = "USER";
        private const string PasswordKey = "PASSWORD";

        private const string SqlServerScheme = "sqlserver";
        private const string SqliteScheme = "sqlite";
        private const string OrientDbScheme = "orientdb";

        private const string ConnectionSchemeDefault = "sqlserver";
        private const string HostNameDefault = "127.0.0.1";
        private const int PortDefault = -1;
        private const string UserNameDefault = "root";
        private const string PasswordDefault = "root";
        private const int ConnectionTimeoutDefault = 15;


        private readonly Dictionary<string, string> _connectionParameters = new Dictionary<string, string>
        {
            { ConnectionUriKey, string.Empty },
            { ConnectionSchemeKey, string.Empty },
            { ConnectionNameKey, string.Empty },
            { ConnectionStringKey, string.Empty },
            { HostNameKey, HostNameDefault },
            { PortKey, PortDefault.ToString() },
            { UserNameKey,  UserNameDefault},
            { PasswordKey, PasswordDefault }
        };

        private readonly Dictionary<string, Action<Uri, StorageConnectionInfo>> _parseConnection = new Dictionary<string, Action<Uri, StorageConnectionInfo>>
        {
            {SqlServerScheme, (uri, conn) =>
                {
                    conn.HostName = uri.Host;
                    conn.Port = uri.Port;
                    conn.ConnectionName = uri.Segments.Last();
                    conn.ConnectionScheme = uri.Scheme;
                    var queryPairs = GetQueryPairs(uri);
                    conn.UpdateParameters(queryPairs);

                    var connectionBuilder = new SqlConnectionStringBuilder
                    {
                        DataSource = conn.HostName + (conn.Port != PortDefault ? $":{conn.Port}" : string.Empty),
                        InitialCatalog = conn.ConnectionName,
                        PersistSecurityInfo = false
                    };

                    if (!string.IsNullOrWhiteSpace(conn.UserName))
                    {
                        connectionBuilder.IntegratedSecurity = false;
                        connectionBuilder.UserID = conn.UserName;
                        connectionBuilder.Password = conn.Password;
                    }
                    else
                    {
                        connectionBuilder.IntegratedSecurity = true;
                    }

                    conn.ConnectionString = connectionBuilder.ToString();
                    conn.Parameters = new ReadOnlyDictionary<string, string>(conn._connectionParameters);
                }
            },
            {OrientDbScheme, (uri, conn) =>
                {
                    conn.HostName = uri.Host;
                    conn.Port = uri.Port != 0 ? uri.Port : 2424;
                    conn.ConnectionName = uri.Segments.Last();
                    conn.ConnectionScheme = uri.Scheme;
                    var queryPairs = GetQueryPairs(uri);
                    conn.UpdateParameters(queryPairs);
                    conn.Parameters = new ReadOnlyDictionary<string, string>(conn._connectionParameters);
                }
            },
            {SqliteScheme, (uri, conn) =>
                {
                    conn.HostName = uri.Host;
                    conn.Port = uri.Port;
                    conn.ConnectionName = uri.Segments.Last();
                    conn.ConnectionScheme = uri.Scheme;
                    var queryPairs = GetQueryPairs(uri);
                    conn.UpdateParameters(queryPairs);
                    conn.Parameters = new ReadOnlyDictionary<string, string>(conn._connectionParameters);
                }
            }
        };

        /// <summary>
        /// Storage connection info constructor.
        /// </summary>
        /// <param name="connectionUri">connection URI</param>
        /// <param name="options">connection options</param>
        public StorageConnectionInfo(string connectionUri, IConnectionOptions? options = null)
        {
            ConnectionUri = connectionUri;
            try
            {
                var uri = new Uri(connectionUri);
                var dbType = uri.Scheme.ToLower();
                if (!_parseConnection.TryGetValue(dbType, out var action)) throw new NotSupportedException($"The database scheme '{dbType} is unsupported.");
                action.Invoke(uri, this);

                HostName = uri.Host;
                Port = uri.Port;
                ConnectionName = uri.Segments.Last();
                ConnectionScheme = uri.Scheme;
                Parameters = new Dictionary<string, string>();
                var queryPairs = GetQueryPairs(uri);
                UpdateParameters(queryPairs);

                foreach (var (key, value) in queryPairs)
                {
                    _connectionParameters.AddOrUpdate(key.ToUpper(), value ?? string.Empty, (k, v) => value ?? string.Empty);
                }
            }
            catch (Exception)
            {
                try
                {
                    // Assume Url string is a SqlServer connection string.
                    var builder = new SqlConnectionStringBuilder(connectionUri);
                    var dataSource = builder.DataSource.Split(':');
                    HostName = dataSource[0];
                    Port = dataSource.Length == 2 ? int.Parse(dataSource[1]) : -1;
                    ConnectionName = builder.InitialCatalog;
                    ConnectionScheme = SqlServerScheme;
                    ConnectionString = builder.ToString();
                    UserName = builder.UserID;
                    Password = builder.Password;
                    Parameters = new ReadOnlyDictionary<string, string>(_connectionParameters);
                }
                catch (Exception)
                {
                    throw new NotSupportedException($"Unsupported string format = '{connectionUri}'.");
                }
            }
        }

        /// <summary>
        /// StorageConnectionInfo constructor.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public StorageConnectionInfo(string databaseName, string hostName, int port, string userName, string password)
        {
            ConnectionName = databaseName;
            HostName = hostName;
            Port = port;
            UserName = userName;
            Password = password;
            Parameters = new Dictionary<string, string>();
            ConnectionUri = $"{ConnectionSchemeDefault}://{HostName}:{Port}/{ConnectionName}?user={UserName}&password={Password}";
        }

        #region Properties

        public string ConnectionUri
        {
            get => _connectionParameters[ConnectionUriKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(ConnectionUriKey, value, (k, v) => value); }
        }

        public string ConnectionName
        {
            get => _connectionParameters[ConnectionNameKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(ConnectionNameKey, value, (k, v) => value); }
        }

        public string ConnectionScheme
        {
            get => _connectionParameters[ConnectionSchemeKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(ConnectionSchemeKey, value, (k, v) => value); }
        }

        public string ConnectionString
        {
            get => _connectionParameters[ConnectionStringKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(ConnectionStringKey, value, (k, v) => value); }
        }

        public int ConnectionTimeout => ConnectionTimeoutDefault;

        public string HostName
        {
            get => _connectionParameters[HostNameKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(HostNameKey, value, (k, v) => value); }
        }

        public int Port
        {
            get => int.Parse(_connectionParameters[PortKey]);
            private set { _connectionParameters.AddOrUpdate(PortKey, value.ToString(), (k, v) => value.ToString()); }
        }


        public string UserName
        {
            get => _connectionParameters[UserNameKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(UserNameKey, value, (k, v) => value); }
        }

        public string Password
        {
            get => _connectionParameters[PasswordKey] ?? string.Empty;
            private set { _connectionParameters.AddOrUpdate(PasswordKey, value, (k, v) => value); }
        }

        public IReadOnlyDictionary<string, string> Parameters { get; private set; }

        public IConnectionOptions Options { get; } = new SqlServerOptions();

        #endregion

        #region Helpers

        /// <summary>
        /// Obtain the query string pairs
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>dictionary of query string pairs</returns>
        private static IDictionary<string, string?> GetQueryPairs(Uri uri)
        {
            var queryString = uri.Query.Split('?');
            var queryPairs = new Dictionary<string, string?>();

            var parts = queryString[1].Split('&');
            foreach (var item in parts)
            {
                var kvItem = item.Split('=');
                var key = kvItem[0].ToUpper();
                var value = kvItem.Length == 2 ? kvItem[1] : null;
                if (queryPairs.ContainsKey(key))
                {
                    queryPairs[key] = value;
                }
                else
                {
                    queryPairs.Add(key, value);
                }
            }

            return queryPairs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        private void UpdateParameters(IDictionary<string, string?> parameters)
        {
            foreach (var (key, value) in parameters)
            {
                if (_connectionParameters.ContainsKey(key))
                {
                    _connectionParameters[key] = value ?? string.Empty;
                }
                else
                {
                    _connectionParameters.Add(key, value ?? string.Empty);
                }
            }
        }

        #endregion
    }
}
