using IdentityServerDemo.ExternalAuth.Interfaces;
using System;
using System.Collections.Generic;

namespace IdentityServerDemo.ExternalAuth
{
    public class ExternalAuthSelector
    {
        private readonly IFacebookAuthProvider _facebookAuthProvider;
        private readonly IGoogleAuthProvider _googleAuthProvider;
        private readonly IAppleAuthProvider _appleAuthProvider;
        private Dictionary<ProviderType, IExternalAuthProvider> _providers;
        public ExternalAuthSelector(IFacebookAuthProvider facebookAuthProvider,
            IGoogleAuthProvider googleAuthProvider, IAppleAuthProvider appleAuthProvider)
        {
            _facebookAuthProvider = facebookAuthProvider ?? throw new ArgumentNullException(nameof(facebookAuthProvider));
            _googleAuthProvider = googleAuthProvider ?? throw new ArgumentNullException(nameof(googleAuthProvider));
            _appleAuthProvider = appleAuthProvider ?? throw new ArgumentNullException(nameof(appleAuthProvider));

            _providers = new Dictionary<ProviderType, IExternalAuthProvider>
            {
                 {ProviderType.Facebook, _facebookAuthProvider},
                 {ProviderType.Google, _googleAuthProvider},
                 {ProviderType.Apple, _appleAuthProvider},
            };
        }

        public IExternalAuthProvider GetProvider(string provider)
        {
            var providerType = (ProviderType)Enum.Parse(typeof(ProviderType), provider, true);

            if (string.IsNullOrWhiteSpace(provider) || !Enum.IsDefined(typeof(ProviderType), providerType))
            {
                throw new Exception("invalid provider");
            }
            return _providers[providerType];
        }

        public IExternalAuthProvider GetProvider(ProviderType providerType)
        {
            return _providers[providerType];
        }
    }
}
