namespace CafeLib.Data.Sources
{
    public interface IConnectionOptions
    {
        ISqlCommandProcessor CommandProcessor { get; }
    }
}
