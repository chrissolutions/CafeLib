using System;
using CafeLib.Core.Dynamic;

namespace CafeLib.Core.UnitTests.DynamicModels
{
    public class ExpandoModel : Expando
    {
        public ExpandoModel()
        { }

        /// <summary>
        /// Allow passing in of an instance
        /// </summary>
        /// <param name="instance"></param>
        public ExpandoModel(object instance)
            : base(instance)
        { }
    }
}