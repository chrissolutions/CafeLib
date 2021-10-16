using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public abstract class AbstractMethodTranslator
    {
        protected readonly IModelInfoProvider _infoProvider;

        protected readonly IDbObjectFactory _dbFactory;

        public AbstractMethodTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory)
        {
            _infoProvider = infoProvider;
            _dbFactory = dbFactory;
        }

        public abstract void Register(TranslationPlugIns plugIns);

        public abstract void Translate(
            MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator);
    }
}