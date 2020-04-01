﻿using System.Text;
using CafeLib.Data.SqlGenerator;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;
using CafeLib.Data.SqlGenerator.Extensions;

namespace CafeLib.Data.Sources.Sqlite.DbObjects
{
    public class SqliteTempTable : SqlTempTable
    {
        public SqliteTempTable()
        {
            RowNumberColumnName = TranslationConstants.SqliteRowNumberColumnAlias;
        }

        public override IDbObject GetCreateStatement(IDbObjectFactory factory)
        {
            string Action()
            {
                var sb = new StringBuilder();

                sb.AppendLine($"create temporary table if not exists {this} as ");
                sb.AppendLineWithSpace(SourceSelect.ToString());
                sb.AppendLine();

                return sb.ToString();
            }

            return new DbDynamicStatement(Action);
        }

        public override IDbObject GetDropStatement(IDbObjectFactory factory)
        {
            string Action()
            {
                var sb = new StringBuilder();
                sb.AppendLine($"drop table if exists {this}");
                return sb.ToString();
            }

            return new DbDynamicStatement(Action);
        }
    }
}