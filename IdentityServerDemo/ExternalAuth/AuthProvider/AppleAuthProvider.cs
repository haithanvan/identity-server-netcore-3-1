using IdentityModel;
using IdentityServerDemo.ExternalAuth.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth.AuthProvider
{
    public class AppleAuthProvider : IAppleAuthProvider
    {
        public async Task<TokenInfo> GetInfoByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decode = handler.ReadJwtToken(token);
            if (await VerifySignature(token))
            {
                return new TokenInfo()
                {
                    UserId = decode.Subject,
                    Email = decode.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value
                };
            }
            return null;
        }

        public async Task<bool> VerifySignature(string token)
        {
            var httpClient = new HttpClient();
            var provider = ProviderDataSource.GetProvider(ProviderType.Apple);
            string jwksUri = $"{provider.TokenInfoEndPoint}/auth/keys";
            var result = await httpClient.GetAsync(jwksUri);
            if (result.IsSuccessStatusCode)
            {
                var jwksContent = JObject.Parse(await result.Content.ReadAsStringAsync());
                var jwks = (JArray)jwksContent["keys"];
                return jwks.Any(item =>
                {
                    JObject jwk = (JObject)item;
                    string[] tokenParts = token.Split('.');
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(
                       new RSAParameters()
                       {
                           Modulus = FromBase64Url(jwk["n"].ToString()),
                           Exponent = FromBase64Url(jwk["e"].ToString())
                       });
                    var validationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = provider.TokenInfoEndPoint,
                        ValidateLifetime = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };

                    var handler = new JwtSecurityTokenHandler();
                    try
                    {
                        handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                });

            }
            return false;
        }

        static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
    }
}
