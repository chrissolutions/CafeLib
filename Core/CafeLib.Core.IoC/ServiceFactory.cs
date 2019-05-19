namespace CafeLib.Core.IoC
{
    //public delegate object ServiceFactory(params object[] args);
    public delegate T ServiceFactory<out T>(params object[] args) where T : class;
}
