using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nmb.Shared.Identity
{
    public class TokenRetriever
    {
        private readonly ClientCredentials _credentials;
        private readonly ILogger<TokenRetriever> _logger;
        private readonly IMemoryCache _cache;
        private const string CachePrefix = "access_token";

        public TokenRetriever(
            ClientCredentials credentials,
            ILogger<TokenRetriever> logger,
            IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
            _credentials = credentials;
        }

        public async Task<string> RequestToken(string scope = null)
        {
            scope = scope ?? string.Join(" ", _credentials.Scopes.OrderBy(t => t));
            var cacheKey = $"{CachePrefix}:{scope}";

            if (_cache.TryGetValue(cacheKey, out string cachedToken))
            {
                return cachedToken;
            }
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _credentials.Authority,
                Policy =
                {
                    RequireHttps = false,
                    ValidateIssuerName = false,
                    ValidateEndpoints = false,
                    LoopbackAddresses = { "localhost", "127.0.0.1", "identity-api" },
                    AllowHttpOnLoopback = true
                }
            });
            if (disco.IsError)
            {
                _logger.LogError("IdServer discovery request failed. Error: {0}", disco.Error);
                return null;
            }
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _credentials.Authority,
                ClientId = _credentials.ClientId,
                ClientSecret = _credentials.Secret,
                Scope = string.Join(" ", _credentials.Scopes)
            });
            if (tokenResponse.IsError)
            {
                _logger.LogError("Get token failed, error: {0}", tokenResponse.Error);
                return null;
            }
            _cache.Set(cacheKey, tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn / 4));
            return tokenResponse.AccessToken;
        }
    }
}
