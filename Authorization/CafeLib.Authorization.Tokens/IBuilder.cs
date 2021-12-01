namespace CafeLib.Authorization.Tokens
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}
