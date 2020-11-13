using Nmb.Shared.Constants;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace Nmb.Shared.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetHlsLicenseUrl(this IConfiguration configuration, string objectKey)
        {
            var basePath = configuration.GetValue<string>(ConfigurationKeys.HlsLicenseUrl);
            return $"{basePath}?key={WebUtility.UrlEncode(objectKey)}";
        }

        public static T GetJsonValue<T>(this IConfiguration configuration, string objectKey)
        {
            try
            {
                var value = configuration.GetValue<string>(objectKey);
                var jsonValue = JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return jsonValue;
            }
            catch
            {
                return default;
            }
        }
    }
}
