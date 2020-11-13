using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nmb.Shared.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Mvc
{
    public static class HttpClientAuthHandlerExtension
    {
        public static IServiceCollection AddTokenRetriever(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMemoryCache()
                .AddSingleton(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>()
                        .GetSection("ClientCredentials")
                        .Get<ClientCredentials>();
                    var logger = sp.GetRequiredService<ILogger<TokenRetriever>>();
                    var cache = sp.GetRequiredService<IMemoryCache>();
                    return new TokenRetriever(config, logger, cache);
                });
            return services;
        }

        /// <summary>
        /// Resolve optional dependency
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientAuthorizationDelegatingHandler(this IServiceCollection services)
        {
            services.AddTransient(sp =>
            {
                var tokenRetriever = sp.GetService<TokenRetriever>();
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                return new HttpClientAuthorizationDelegatingHandler(httpContextAccessor, tokenRetriever);
            });
            return services;
        }
    }
}
