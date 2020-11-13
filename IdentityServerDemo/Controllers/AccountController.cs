using Hangfire;
using IdentityServerDemo.Domain.AccountAggregate;
using IdentityServerDemo.ExternalAuth;
using IdentityServerDemo.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nmb.Shared.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IScopeContext _scopeContext;
        private readonly UserManager<Account> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly IBackgroundJobClient _jobClient;
        private readonly ExternalAuthSelector _externalAuthSelector;

        public AccountController(IScopeContext scopeContext, UserManager<Account> userManager, ILogger<AccountController> logger, IStringLocalizer<AccountController> localizer, IBackgroundJobClient jobClient, ExternalAuthSelector externalAuthSelector)
        {
            _scopeContext = scopeContext;
            _userManager = userManager;
            _logger = logger;
            _localizer = localizer;
            _jobClient = jobClient;
            _externalAuthSelector = externalAuthSelector;
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning(_localizer["No user found with this email"]);
                return BadRequest(_localizer["No user found with this email"]);
            }
            if (!user.IsActive)
            {
                _logger.LogWarning(_localizer["User account is not active"]);
                return BadRequest(_localizer["User account is not active"]);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            //_jobClient.Enqueue<IEmailTask>(t => t.SendForgotPasswordEmail(user.Email, user.FullName, code));
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning(_localizer["No user found with this email"]);
                return BadRequest(_localizer["No user found with this email"]);
            }
            if (!user.IsActive)
            {
                _logger.LogWarning(_localizer["User account is not active"]);
                return BadRequest(_localizer["User account is not active"]);
            }
            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else return BadRequest(_localizer["Reset password error"]);

        }

        [AllowAnonymous]
        [HttpPost("info/token")]
        public async Task<IActionResult> GetUserInfoByToken(GetInfoByTokenRequest request)
        {
            var tokenInfo = await _externalAuthSelector.GetProvider(request.Provider).GetInfoByToken(request.Token);
            if (tokenInfo != null)
            {
                var users = _userManager.Users.SelectMany(t => t.Logins).Where(t =>
                    t.LoginProvider == request.Provider && t.ProviderKey == tokenInfo.UserId).ToList();
                if (users.Count > 0)
                {
                    return Ok(new
                    {
                        IsAssociate = true
                    });
                }
                else
                {
                    return Ok(new
                    {
                        tokenInfo.Email,
                        IsAssociate = false
                    });
                }
            }
            return BadRequest("Invalid external token");
        }

    }
}
