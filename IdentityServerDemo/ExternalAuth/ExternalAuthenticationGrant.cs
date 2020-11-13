using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityServerDemo.Domain.AccountAggregate;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth
{
    public class ExternalAuthenticationGrant : IExtensionGrantValidator
    {
        private readonly UserManager<Account> _userManager;
        private readonly ExternalAuthSelector _externalAuthSelector;

        public ExternalAuthenticationGrant(UserManager<Account> userManager,
            ExternalAuthSelector externalAuthSelector)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _externalAuthSelector = externalAuthSelector;
        }

        public string GrantType => "external";
        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {

            var token = context.Request.Raw.Get("token");
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "invalid external token");
                return;
            }
            var requestEmail = context.Request.Raw.Get("email");
            var provider = context.Request.Raw.Get("provider");
            var tokenInfo = await _externalAuthSelector.GetProvider(provider).GetInfoByToken(token);

            if (tokenInfo == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "couldn't retrieve user info from specified provider, please make sure that access token is not expired.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(tokenInfo.UserId))
            {

                var user = _userManager.Users.FirstOrDefault(t => t.Logins.Any(p => p.LoginProvider == provider && p.ProviderKey == tokenInfo.UserId));
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        user = await _userManager.FindByIdAsync(user.Id.ToString());
                        var userClaims = await _userManager.GetClaimsAsync(user);
                        context.Result = new GrantValidationResult(user.Id.ToString(), provider, userClaims, provider, null);
                        return;
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "could not retrieve user Id from the token provided");
                    }
                }
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "could not retrieve user Id from the token provided");
            return;
        }
    }
}
