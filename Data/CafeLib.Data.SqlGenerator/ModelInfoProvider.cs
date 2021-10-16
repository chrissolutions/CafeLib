using System;
using System.Reflection;

namespace CafeLib.Data.SqlGenerator
{
    public interface IModelInfoProvider
    {
        EntityInfo FindEntityInfo(Type type);
        EntityFieldInfo FindFieldInfo(MemberInfo memberInfo);
    }
}