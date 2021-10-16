using System;
using System.Collections.Generic;
using System.Text;

namespace CafeLib.Core.UnitTests.TypeModels
{
    public class TypeWithSingleParameterConstructor
    {
        public int Argument { get; }

        public TypeWithSingleParameterConstructor(int argument)
        {
            Argument = argument;
        }
    }
}
