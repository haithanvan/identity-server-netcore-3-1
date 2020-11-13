using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Identity
{
    public class CustomTokenRetriever
    {
        public static Func<HttpRequest, string> FromHeaderAndQueryString(string headerScheme = "Bearer", string queryScheme = "access_token")
        {
            return (request) =>
            {
                var token = TokenRetrieval.FromAuthorizationHeader(headerScheme)(request);
                return !string.IsNullOrEmpty(token) ? token : TokenRetrieval.FromQueryString(queryScheme)(request);
            };
        }
    }
}
