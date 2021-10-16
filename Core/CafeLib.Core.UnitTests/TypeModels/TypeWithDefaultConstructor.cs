using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.Core.UnitTests.TypeModels
{
    public class TypeWithDefaultConstructor
    {
        public int Default { get; }

        public TypeWithDefaultConstructor()
        {
            Default = 100;
        }
    }
}
