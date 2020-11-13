using IdentityServerDemo.ExternalAuth.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth.AuthProvider
{
    public class GoogleAuthProvider : IGoogleAuthProvider
    {

        public async Task<TokenInfo> GetInfoByToken(string token)
        {
            var httpClient = new HttpClient();
            var provider = ProviderDataSource.GetProvider(ProviderType.Google);

            var result = await httpClient.GetAsync($"{provider.TokenInfoEndPoint}{token}");
            if (result.IsSuccessStatusCode)
            {
                var infoObject = JObject.Parse(await result.Content.ReadAsStringAsync());
                return new TokenInfo()
                {
                    UserId = infoObject.Value<string>("sub"),
                    Email = infoObject.Value<string>("email")
                };
            }
            return null;
        }
    }
}

