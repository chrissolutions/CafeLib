using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CafeLib.Data.Expressions
{
    internal class SimpleTypeBinder
    {
        public Dictionary<string, PropertyInfo> GetProperties(Type t)
        {
            return t.GetProperties().Where(x => x.CanWrite && IsSimpleType(x.PropertyType)).ToDictionary(x => x.Name);
        }

        private static bool IsSimpleType(Type t)
        {
            while (true)
            {
                if (t.IsPrimitive)
                    return true;

                if (t.IsEnum)
                    return true;

                if (t == typeof(Guid))
                    return true;

                if (t == typeof(decimal))
                    return true;

                if (t == typeof(string))
                    return true;

                if (t == typeof(DateTime))
                    return true;

                if (t == typeof(byte[]))
                    return true;

                t = Nullable.GetUnderlyingType(t);
                if (t == null)
                {
                    break;
                }
            }
            return false;
        }
    }
}