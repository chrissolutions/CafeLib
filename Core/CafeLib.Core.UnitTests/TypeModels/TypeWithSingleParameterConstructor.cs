namespace CafeLib.Core.UnitTests.TypeModels
{
    public class TypeWithParametersConstructor
    {
        public int Argument1 { get; }
        public int Argument2 { get; }

        public TypeWithParametersConstructor(int argument)
        {
            Argument1 = argument;
        }

        public TypeWithParametersConstructor(int arg1, int arg2)
            : this(arg1)
        {
            Argument2 = arg2;
        }
    }
}
