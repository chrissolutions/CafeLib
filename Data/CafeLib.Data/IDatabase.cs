namespace CafeLib.Data
{
    public interface IDatabase<out T> where T : IStorage
    {
        string DatabaseName { get; }
        string ConnectionString { get; }
        T GetStorage();
    }

    public interface IDatabase : IDatabase<IStorage>
    {
    }
}
