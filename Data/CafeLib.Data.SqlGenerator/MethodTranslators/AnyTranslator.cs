﻿using System.Linq;
using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.Extensions;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public class AnyTranslator : AbstractMethodTranslator
    {
        public AnyTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory) 
            : base(infoProvider, dbFactory)
        {
        }

        public override void Register(TranslationPlugIns plugIns)
        {
            plugIns.RegisterMethodTranslator("any", this);
        }

        public override void Translate(
            MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator)
        {
            // there may not be a preidcate in any method call
            IDbObject condition = null;
            if (m.GetArguments().Any())
                condition = state.ResultStack.Pop();

            var childSelect = (IDbSelect)state.ResultStack.Pop();
            var dbSelect = (IDbSelect)state.ResultStack.Peek();

            childSelect.Where = condition.ToBinary(_dbFactory);

            var dbJoin = dbSelect.Joins.Single(j => ReferenceEquals(j.To.Referee, childSelect));

            IDbBinary whereClause = null;

            foreach(var joinKey in dbJoin.Condition.GetOperands().OfType<IDbColumn>().Where(c => ReferenceEquals(c.Ref, dbJoin.To)))
            {
                var pkColumn = _dbFactory.BuildColumn(dbJoin.To, joinKey.Name, joinKey.ValType.DotNetType, joinKey.Alias);
                var binary = _dbFactory.BuildBinary(pkColumn, DbOperator.IsNot, _dbFactory.BuildConstant(null));

                whereClause = whereClause.UpdateBinary(binary, _dbFactory);
            }

            state.ResultStack.Push(whereClause);
        }
    }
}