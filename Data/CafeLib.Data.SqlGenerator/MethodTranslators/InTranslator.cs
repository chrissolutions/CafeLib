﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public class InTranslator : AbstractMethodTranslator
    {
        public InTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory)
            : base(infoProvider, dbFactory)
        {
        }

        public override void Register(TranslationPlugIns plugIns)
        {
            plugIns.RegisterMethodTranslator("in", this);
        }

        public override void Translate(
            MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator)
        {
            var vals = new List<object>();
            while (state.ResultStack.Peek() is IDbConstant)
            {
                var dbConstants = (IDbConstant)state.ResultStack.Pop();
                if (dbConstants.Val is IEnumerable val)
                {
                    vals = val.Cast<object>().ToList();
                    break;
                }
                vals.Insert(0, dbConstants.Val);
            }

            var dbExpression = (IDbSelectable)state.ResultStack.Pop();
            var dbBinary = vals.Count == 0
                ? _dbFactory.BuildBinary(_dbFactory.BuildConstant(0), DbOperator.Equal, _dbFactory.BuildConstant(1))
                : _dbFactory.BuildBinary(dbExpression, DbOperator.In, _dbFactory.BuildConstant(vals, true));

            state.ResultStack.Push(dbBinary);
        }
    }
}