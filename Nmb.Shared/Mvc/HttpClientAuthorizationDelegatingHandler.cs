using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Nmb.Shared.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nmb.Shared.Mvc
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenRetriever _tokenRetriever;

        public HttpClientAuthorizationDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            TokenRetriever tokenRetriever)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenRetriever = tokenRetriever;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await SetToken(request, cancellationToken);
            SetLanguage(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private void SetLanguage(HttpRequestMessage request)
        {
            if (_httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Language",
                    out var acceptLangHeader) && !string.IsNullOrEmpty(acceptLangHeader))
            {
                request.Headers.Add("Accept-Language", new List<string>() { acceptLangHeader });
            }
        }

        private async Task SetToken(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                await DelegateAccessToken(request);
                return;
            }

            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == false)
            {
                if (_tokenRetriever == null)
                    return;

                var clientToken = await _tokenRetriever?.RequestToken();
                if (!string.IsNullOrEmpty(clientToken))
                    request.SetBearerToken(clientToken);
            }
        }

        private async Task DelegateAccessToken(HttpRequestMessage request)
        {
            if (!request.Headers.Contains("Authorization") && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization",
                    out var authorizationHeader) && !string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
                return;
            }

            var token = await GetToken();
            if (token != null)
            {
                request.SetBearerToken(token);
            }
        }


        private async Task<string> GetToken()
        {
            const string accessToken = "access_token";
            return await _httpContextAccessor.HttpContext.GetTokenAsync(accessToken);
        }
    }
}
