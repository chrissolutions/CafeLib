namespace CafeLib.Data.Options
{
    public interface IConnectionOptions
    {
        ISqlCommandProcessor CommandProcessor { get; }
    }
}
