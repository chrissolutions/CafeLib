using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public class LimitTranslator : AbstractMethodTranslator
    {
        public LimitTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory) 
            : base(infoProvider, dbFactory)
        {
        }

        public override void Register(TranslationPlugIns plugIns)
        {
            plugIns.RegisterMethodTranslator("take", this);
            plugIns.RegisterMethodTranslator("skip", this);
        }

        public override void Translate(MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator)
        {
            throw new System.NotImplementedException();
        }
    }
}