using System.Collections.Generic;

namespace Nmb.Shared.Utils
{
    public static class ScssUtils
    {
        public static IEnumerable<string> GenerateScssVariables(object data)
        {
            var result = new List<string>();
            foreach (var property in data.GetType().GetProperties())
            {
                // if (property.PropertyType == typeof(Font))
                // {
                //     continue;
                // }
                // var value = property.GetValue(data);
                // if (value == null)
                // {
                //     continue;
                // }
                // if (property.PropertyType.IsEnum)
                // {
                //     var description = value.GetEnumDescription();
                //     result.Add($"${property.Name.ToKebabCasing()}: {description};");
                // }
                // else if (property.PropertyType == typeof(string) && property.GetCustomAttribute<UrlCustomAttribute>() != null)
                // {
                //     result.Add($"${property.Name.ToKebabCasing()}: '{value}';");
                // }
                // else
                // {
                //     result.Add($"${property.Name.ToKebabCasing()}: {value.ToString().ToLower()};");
                // }
            };
            return result;
        }
    }
}
