using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public class WhereTranslator : AbstractMethodTranslator
    {
        public WhereTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory) 
            : base(infoProvider, dbFactory)
        {
        }

        public override void Register(TranslationPlugIns plugIns)
        {
            plugIns.RegisterMethodTranslator("where", this);
        }

        public override void Translate(MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator)
        {
            var dbElement = state.ResultStack.Pop();
            var whereClause = dbElement.ToBinary(_dbFactory);
                
            var dbSelect = (IDbSelect)state.ResultStack.Peek();
            dbSelect.UpdateWhereClause(whereClause, _dbFactory);
        }   
    }
}