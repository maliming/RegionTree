namespace RegionTree
{
    public static class StringExtensions
    {
        public static bool Is2Root(this string str)
        {
            return str.EndsWith("0000");
        }

        public static bool Is3Root(this string str)
        {
            return str.EndsWith("00") && !str.Is2Root();
        }
    }
}