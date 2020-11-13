using System.Collections.Generic;

namespace Nmb.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this IDictionary<string, T> dict, string key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            
            key = key.ToCamelCasing();
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }

            return default;
        }
    }
}
