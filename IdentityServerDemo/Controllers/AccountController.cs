using Hangfire;
using IdentityServerDemo.Commands;
using IdentityServerDemo.Domain.AccountAggregate;
using IdentityServerDemo.ExternalAuth;
using IdentityServerDemo.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nmb.Shared.Constants;
using Nmb.Shared.Exceptions;
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

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(CreateAccountCommand command)
        {
            var account = Account.CreateNewAccount(command.Email, command.FirstName, command.LastName, command.IsActive, command.ProfileImageUrl);
            var result = await _userManager.CreateAsync(account, command.Password);
            if (!result.Succeeded)
            {
                return Ok(new
                {
                    Result = result
                });
            }

            result = await _userManager.AddToRoleAsync(account, command.Role);
            return Ok(new
            {
                Result = result,
                AccountId = account.Id,
            });
        }

        [HttpPost("confirmSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmSignUp(ConfirmSignupCommand request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, request.Otp);
                if (result.Succeeded)
                {
                    user.UpdateStatus(true);
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    throw new Exception("ConfirmEmailError");
                }
            }
            return Ok();
        }

        [HttpPost("resendSignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendSignUp([FromBody] ResendSignupRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.ToLower());
            if (user != null)
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //_jobClient.Enqueue<IEmailTask>(t => t.SendOTPEmail(user.Email, user.FullName, code));
            }
            return Ok();
        }

        [HttpPost("externalRegister")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalRegister(RegisterExternalUserRequest request)
        {
            if (ModelState.IsValid)
            {
                var tokenInfo = await _externalAuthSelector.GetProvider(request.Provider).GetInfoByToken(request.Token);
                if (tokenInfo == null)
                {
                    return BadRequest("Invalid external token");
                }
                var user = await CreateNewUserIdentityAsync(request.Email.ToLower(), string.Empty, request.FirstName, request.LastName, request.PhoneNumber,request.ProfileImageUrl,request.Address, true);
                if (user == null)
                {
                    throw new Exception("CreateIdentityUserFailed");
                }
                await _userManager.AddToRoleAsync(user, AllRoles.Mobile);
                await _userManager.AddLoginAsync(user, new UserLoginInfo(request.Provider, tokenInfo.UserId, request.Provider));
                return Ok();
            }
            return BadRequest(ModelState);
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
            return Ok(code);
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

        private async Task<Account> CreateNewUserIdentityAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber,
            string profileImageUrl,
            string address,
            bool confirmEmail,
            bool isActive = true)
        {
            var existsUser = await _userManager.FindByEmailAsync(email);
            if (existsUser != null)
            {
                throw new Exception("UserEmailExisted");
            }
            var user = new Account(email, firstName, lastName, profileImageUrl, phoneNumber, address, isActive);
            if (confirmEmail)
            {
                user.ConfirmEmail();
            }
            IdentityResult result;
            if (string.IsNullOrEmpty(password))
            {
                result = await _userManager.CreateAsync(user);
            }
            else
            {
                result = await _userManager.CreateAsync(user, password);
            }
            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }
    }
}
