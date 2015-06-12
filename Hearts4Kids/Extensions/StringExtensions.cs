using System.Text.RegularExpressions;

namespace Hearts4Kids.Extensions
{
    public static class StringExtensions
    {
        public static string SplitCamelCase(this string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1").Trim();
        }
    }
}