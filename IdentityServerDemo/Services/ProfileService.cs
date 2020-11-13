using AspNet.Security.OpenIdConnect.Primitives;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerDemo.Domain.AccountAggregate;
using Microsoft.AspNetCore.Identity;
using Nmb.Shared.Constants;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerDemo.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<Account> _userManager;
        private readonly IUserClaimsPrincipalFactory<Account> _claimsFactory;

        public ProfileService(UserManager<Account> userManager, IUserClaimsPrincipalFactory<Account> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.Claims.Where(x => x.Type == "sub").FirstOrDefault().Value;

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");
            var principal = await _claimsFactory.CreateAsync(user);
            var reqClaimTypes = context.RequestedClaimTypes.Concat(new[]
            {
                OpenIdConnectConstants.Claims.Role,
            });

            var claims = principal.Claims.Where(c => reqClaimTypes.Contains(c.Type)).ToList();
            claims.AddRange(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(OpenIdConnectConstants.Claims.Username, $"{user.FirstName ?? string.Empty} {user.LastName ?? string.Empty}"),
                new Claim(JwtClaimTypes.FamilyName, user.LastName ?? string.Empty),
                new Claim(JwtClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber ?? string.Empty),
                new Claim(JwtClaimTypes.Picture, user.ProfileImageUrl ?? string.Empty),
                new Claim(JwtClaimTypes.Address, user.Address ?? string.Empty),
            });

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            }
            if (user.ProfileImageUrl != null)
            {
                var profileImageUrl = user.ProfileImageUrl;
                claims.Add(new Claim(JwtClaimTypes.Picture, profileImageUrl));
            }
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            if (user == null)
            {
                context.IsActive = false;
                return;
            }
            var clientId = context.Client.ClientId;
            var roleNames = await _userManager.GetRolesAsync(user);

            if (IsClientAllowed(roleNames, clientId))
            {
                context.IsActive = user?.IsActive ?? false;
            }
            else
            {
                context.IsActive = false;
            }
        }

        private bool IsClientAllowed(IList<string> roleNames, string clientId)
        {
            return
                   clientId == IdentityConfiguration.WellknownClientId.Mobile && roleNames.Contains(AllRoles.Learner) ||
                   clientId == IdentityConfiguration.WellknownClientId.Learner && roleNames.Contains(AllRoles.Learner) ||
                   clientId == IdentityConfiguration.WellknownClientId.Admin && roleNames.Contains(AllRoles.Administrator);
        }
    }
}

