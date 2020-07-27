using System;
using CafeLib.Core.Dynamic;

namespace CafeLib.Core.UnitTests.DynamicModels
{
    public class ExpandoInstance : Expando
    {
        public string Name { get; set; }
        public DateTime Entered { get; set; }

        public ExpandoInstance()
        { }

        /// <summary>
        /// Allow passing in of an instance
        /// </summary>
        /// <param name="instance"></param>
        public ExpandoInstance(object instance)
            : base(instance)
        { }
    }
}