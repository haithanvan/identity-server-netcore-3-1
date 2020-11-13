using IdentityServerDemo.ExternalAuth.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth.AuthProvider
{
    public class FacebookAuthProvider : IFacebookAuthProvider
    {

        public async Task<TokenInfo> GetInfoByToken(string token)
        {
            var httpClient = new HttpClient();
            var provider = ProviderDataSource.GetProvider(ProviderType.Facebook);
            var result = await httpClient.GetAsync($"{provider.UserInfoEndPoint}?fields=id,email,name,gender,birthday&access_token={token}");
            if (result.IsSuccessStatusCode)
            {
                var infoObject = JObject.Parse(await result.Content.ReadAsStringAsync());
                return new TokenInfo()
                {
                    UserId = infoObject.Value<string>("id"),
                    Email = infoObject.Value<string>("email")
                };
            }
            return null;
        }
    }
}
