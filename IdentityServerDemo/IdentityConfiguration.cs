using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo
{
    public static class IdentityConfiguration
    {
        public static class WellknownClientId
        {
            public const string Admin = "admin";
            public const string Learner = "learner";
            public const string Mobile = "mobile";
        }

        public static IEnumerable<IdentityResource> IdentityResources =>
           new List<IdentityResource>
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
           };

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope("api", "Api Scope"),
        };

        public static IEnumerable<ApiResource> GetApis(IDictionary<string, string[]> apiSecrets)
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "api",
                    DisplayName = "API service",
                    Description = "Access to core API",
                    ApiSecrets = apiSecrets["Api"].Select(t => new Secret(t.Sha256())).ToList(),
                    Scopes = {"api"}
                }
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, string[]> clientsUrl, Dictionary<string, string> clientSecrets)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = WellknownClientId.Admin,
                    ClientName = "Administration",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(clientSecrets["Admin"].Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris =  clientsUrl["Admin"].Map(t => t + "/oidc/callback"),
                    PostLogoutRedirectUris = clientsUrl["Admin"].Map(t => t + "/oidc/logout-callback"),
                    AllowedCorsOrigins = clientsUrl["Admin"],
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },
                new Client
                {
                    ClientId = WellknownClientId.Learner,
                    ClientName = "Learner Portal",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret(clientSecrets["Learner"].Sha256())
                    },
                    AllowedGrantTypes =
                    {
                        GrantType.ResourceOwnerPassword,
                        "external"
                    },
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris =  clientsUrl["Learner"].Map(t => t + "/oidc/callback"),
                    PostLogoutRedirectUris = clientsUrl["Learner"].Map(t => t + "/oidc/logout-callback"),
                    AllowedCorsOrigins = clientsUrl["Learner"],
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                },
                 new Client
                {
                    ClientId = WellknownClientId.Mobile,
                    ClientName = "Mobile Portal",
                    AllowedGrantTypes =
                    {
                        GrantType.ResourceOwnerPassword,
                        "external"
                    },
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireClientSecret = false,
                    RedirectUris =  clientsUrl["Learner"].Map(t => t + "/oidc/callback"),
                    PostLogoutRedirectUris = clientsUrl["Learner"].Map(t => t + "/oidc/logout-callback"),
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Email,
                        "api",
                    }
                }
            };
        }

    }

    internal static class UriHelper
    {
        internal static string[] Map(this string[] source, Func<string, string> selector)
        {
            return source.Select(selector).ToArray();
        }
    }
}
