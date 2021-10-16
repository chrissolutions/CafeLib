﻿namespace CafeLib.Data.SqlGenerator
{
    public static class TranslationConstants
    {
        public const string JoinKeySuffix = "_jk";
        public const string SubSelectPrefix = "sq";

        public const string MySqlRowNumberColumnAlias = "_ef_row_number_";
        public const string SqliteRowNumberColumnAlias = "_rowid_";
        public const string PostgresQlRowNumberColumnAlias = "ctid";

        public const string TempTablePreix = "Temp_Table_";
    }
}