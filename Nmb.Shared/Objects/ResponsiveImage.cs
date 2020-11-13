using System.Collections.Generic;

namespace Nmb.Shared.Objects
{
    public static class ResponsiveImage
    {
        public static IDictionary<string, string> GetResponsiveImageUrls(this string url)
        {
            return new Dictionary<string, string>
            {
                { "default", url },
                { "800", $"{url}?w=800" },
                { "1024", $"{url}?w=1024" },
                { "1920", $"{url}?w=1920" },
            };
        }
    }
}
