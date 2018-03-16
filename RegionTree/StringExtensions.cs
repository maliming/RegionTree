namespace RegionTree
{
    public static class StringExtensions
    {
        public static bool Is2Root(this string str)
        {
            return str.Contains("0000");
        }

        public static bool Is3Root(this string str)
        {
            return str.Contains("00") && !str.Is2Root();
        }
    }
}