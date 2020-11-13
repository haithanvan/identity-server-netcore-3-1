using IdentityServer4.Validation;
using IdentityServerDemo.ExternalAuth.AuthProvider;
using IdentityServerDemo.ExternalAuth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerDemo.ExternalAuth
{
    public static class ProviderServiceCollectionExtensions
    {
        public static IServiceCollection AddExternalAuth(this IServiceCollection services)
        {
            services.AddTransient<IFacebookAuthProvider, FacebookAuthProvider>();
            services.AddTransient<IGoogleAuthProvider, GoogleAuthProvider>();
            services.AddTransient<IAppleAuthProvider, AppleAuthProvider>();
            services.AddScoped<IExtensionGrantValidator, ExternalAuthenticationGrant>();
            services.AddScoped<ExternalAuthSelector>();
            return services;
        }
    }
}
