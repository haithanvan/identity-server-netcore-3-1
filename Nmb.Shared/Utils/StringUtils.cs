using System;
using System.Text.RegularExpressions;

namespace Nmb.Shared.Utils
{
    public static class StringUtils
    {
        public static bool Contains(this string source, string compareString, StringComparison comp)
        {
            return source?.IndexOf(compareString, comp) >= 0;
        }

        public static string ToTsQueryFormat(string value)
        {
            var space = new Regex(@"\s+");
            return $"{space.Replace(value.Trim(), "&")}";
        }
    }
}
