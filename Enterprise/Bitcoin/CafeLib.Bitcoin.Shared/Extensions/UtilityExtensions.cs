namespace CafeLib.Bitcoin.Shared.Extensions
{
    public static class UtilityExtensions
    {
        public static byte[] Utf8ToBytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s);
    }
}
