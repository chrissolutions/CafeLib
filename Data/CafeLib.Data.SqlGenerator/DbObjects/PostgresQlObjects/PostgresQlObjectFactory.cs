﻿using System;
using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects.SqlObjects;

namespace CafeLib.Data.SqlGenerator.DbObjects.PostgresQlObjects
{
    public class PostgresQlObjectFactory : SqlObjectFactory
    {
        public PostgresQlObjectFactory()
        {
            OutputOption = new DbOutputOption
            {
                ForceQuotationMark = true,
                QuotationMark = "\""
            };
        }

        public override IDbFunc BuildFunc(string name, bool isAggregation, Type type, params IDbObject[] parameters)
        {
            return new PostgresQlFunc(name, type, parameters)
            {
                IsAggregation = isAggregation,
                OutputOption = OutputOption
            };
        }

        public override IDbStatment BuildStatement(IDbObject script)
        {
            return new PostgresQlStatement(script);
        }

        public override IDbTable BuildTable(EntityInfo entityInfo)
        {
            var sqlTable = base.BuildTable(entityInfo);
            
            if (string.IsNullOrEmpty(entityInfo.Namespace))
            {    
                sqlTable.Namespace = "public";
            }

            return sqlTable;
        }
        
        public override IDbTempTable BuildTempTable(string tableName, IDbSelect sourceSelect = null)
        {
            return new PostgresQlTempTable
            {
                TableName = tableName,
                SourceSelect = sourceSelect,
                OutputOption = OutputOption
            };
        }

        public override IDbConstant BuildConstant(object val, bool asParams = false)
        {
            return new PostgresQlConstant
            {
                AsParam = asParams,
                ValType = val == null ? null : BuildType(val.GetType()),
                Val = val
            };
        }

        public override DbLimit BuildLimit(int fetch, int offset = 0)
        {
            return new PostgresQlLimit(offset, fetch);
        }

        public override DbOperator GetDbOperator(ExpressionType eType, Type tl, Type tr)
        {
            var dbOptr = base.GetDbOperator(eType, tl, tr);

            if (dbOptr == DbOperator.Add
                && (tl == typeof(string) || tr == typeof(string)))
            {
                dbOptr = DbOperator.StringAdd;
            }
            
            return dbOptr;
        }
    }
}