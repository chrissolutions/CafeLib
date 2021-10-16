using System.Linq;
using System.Linq.Expressions;
using CafeLib.Data.SqlGenerator.DbObjects;
using CafeLib.Data.SqlGenerator.Extensions;

namespace CafeLib.Data.SqlGenerator.MethodTranslators
{
    public class SelectTranslator : AbstractMethodTranslator
    {
        public SelectTranslator(IModelInfoProvider infoProvider, IDbObjectFactory dbFactory) 
            : base(infoProvider, dbFactory)
        {
        }

        public override void Register(TranslationPlugIns plugIns)
        {
            plugIns.RegisterMethodTranslator("select", this);
        }

        public override void Translate(
            MethodCallExpression m, TranslationState state, UniqueNameGenerator nameGenerator)
        {
            var arguments = state.ResultStack.Pop();
            var dbSelect = (IDbSelect)state.ResultStack.Pop();

            var selections = SqlTranslationHelper.ProcessSelection(arguments, _dbFactory);
            foreach(var selectable in selections)
            {
                SqlTranslationHelper.UpdateJoinType(selectable.Ref);
                dbSelect.Selection.Add(selectable);
            }

            var newSelectRef = _dbFactory.BuildRef(dbSelect);
            var newSelect = _dbFactory.BuildSelect(newSelectRef);

            newSelectRef.Alias = nameGenerator.GenerateAlias(dbSelect, TranslationConstants.SubSelectPrefix, true);

            selections = selections.Concat(dbSelect.Selection.Where(s => s.IsJoinKey)).ToArray();
            foreach(var selectable in selections)
            {
                var newSelectable = CreateNewSelectableForWrappingSelect(
                    dbSelect, selectable, newSelectRef, m, _dbFactory, nameGenerator);

                newSelect.Selection.Add(newSelectable);
            }

            state.ResultStack.Push(newSelect);
        }

        private static IDbSelectable CreateNewSelectableForWrappingSelect(
            IDbSelect dbSelect, IDbSelectable selectable, DbReference dbRef, Expression m, 
            IDbObjectFactory dbFactory, UniqueNameGenerator nameGenerator)
        {
            if (dbRef == null)
                return selectable;

            var oCol =  selectable as IDbColumn;
            if (oCol != null)
                return dbFactory.BuildColumn(dbRef, oCol.GetAliasOrName(), oCol.ValType);

            var oRefCol = selectable as IDbRefColumn;
            if (oRefCol != null)
                return dbFactory.BuildRefColumn(dbRef, oRefCol.Alias, oRefCol);

            if (selectable is IDbFunc oDbFunc)
            {
                if (string.IsNullOrEmpty(oDbFunc.Alias)) 
                    oDbFunc.Alias = nameGenerator.GenerateAlias(dbSelect, oDbFunc.Name, true);
                    
                return dbFactory.BuildColumn(dbRef, oDbFunc.Alias, oDbFunc.ReturnType);
            }

            return dbFactory.BuildColumn(dbRef, selectable.Alias, typeof(string));
        }
    }
}