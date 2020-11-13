using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth
{
    public class ProviderData
    {
        public string Name { get; set; }

        public string UserInfoEndPoint { get; set; }

        public string TokenEndPoint { get; set; }

        public string TokenInfoEndPoint { get; set; }
    }

    public class ProviderDataSource
    {
        public static ProviderData GetProvider(ProviderType providerType)
        {
            return GetProviders()[providerType];
        }

        private static Dictionary<ProviderType, ProviderData> GetProviders()
        {
            return new Dictionary<ProviderType, ProviderData>
            {
                {
                    ProviderType.Facebook, new ProviderData()
                    {
                        Name = "Facebook",
                        UserInfoEndPoint = "https://graph.facebook.com/v2.8/me"
                    }
                },
                {
                    ProviderType.Google, new ProviderData()
                    {
                        Name = "Google",
                        UserInfoEndPoint = "https://www.googleapis.com/oauth2/v2/userinfo",
                        TokenEndPoint = "https://oauth2.googleapis.com/token",
                        TokenInfoEndPoint = "https://oauth2.googleapis.com/tokeninfo?id_token="
                    }
                },
                {
                    ProviderType.Apple, new ProviderData()
                    {
                        Name = "Apple",
                        TokenEndPoint = "https://appleid.apple.com/auth/token",
                        TokenInfoEndPoint = "https://appleid.apple.com",
                    }
                }
            };

        }
    }
}
