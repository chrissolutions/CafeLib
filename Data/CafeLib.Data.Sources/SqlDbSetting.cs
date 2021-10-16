using System;
using RepoDb.Interfaces;

namespace CafeLib.Data.Sources
{
    /// <summary>
    /// A setting class used for IDbConnection data provider.
    /// </summary>
    public sealed class SqlDbSetting : IDbSetting
    {
        public bool AreTableHintsSupported { get; set; }
        public Type AverageableType { get; set; } = typeof(double);
        public string ClosingQuote { get; set; } = "]";
        public string DefaultSchema { get; set; }
        public bool IsDirectionSupported { get; set; }
        public bool IsExecuteReaderDisposable { get; set; } = true;
        public bool IsMultiStatementExecutable { get; set; } = true;
        public bool IsPreparable { get; set; } = true;
        public bool IsUseUpsert { get; set; } = true;
        public string OpeningQuote { get; set; } = "[";
        public string ParameterPrefix { get; set; } = "@";
        public string SchemaSeparator { get; set; } = ".";

        /// <summary>
        /// SqlDbSetting default constructor.
        /// </summary>
        public SqlDbSetting()
        {
        }

        /// <summary>
        /// SqlDbSetting constructor.
        /// </summary>
        public SqlDbSetting(IDbSetting setting)
        {
            AreTableHintsSupported = setting.AreTableHintsSupported;
            AverageableType = setting.AverageableType;
            ClosingQuote = setting.ClosingQuote;
            DefaultSchema = setting.DefaultSchema;
            IsDirectionSupported = setting.IsDirectionSupported;
            IsExecuteReaderDisposable = setting.IsExecuteReaderDisposable;
            IsMultiStatementExecutable = setting.IsMultiStatementExecutable;
            IsPreparable = setting.IsPreparable;
            IsUseUpsert = setting.IsUseUpsert;
            OpeningQuote = setting.OpeningQuote;
            ParameterPrefix = setting.ParameterPrefix;
            SchemaSeparator = setting.SchemaSeparator;
        }
    }
}
