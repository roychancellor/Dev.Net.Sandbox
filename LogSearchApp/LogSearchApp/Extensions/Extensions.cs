namespace LogSearchApp.Extensions
{
    public static class Extensions
    {
        public static string PrefixWith(this string toPrefix, string prefix)
        {
            return $"{prefix} | {toPrefix}";
        }
    }
}
