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
using Nmb.Shared.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace IdentityServerDemo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RemoteController : ControllerBase
    {
        private string CurrentAccountId => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        private readonly UserManager<Account> _userManager;
        private readonly IPasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        private readonly IStringLocalizer<RemoteController> _localizer;

        public RemoteController(UserManager<Account> userManager, IStringLocalizer<RemoteController> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpPost("account")]
        public async Task<IActionResult> CreateAccount(CreateAccountCommand command)
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

        [HttpPost("account/default")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDefaultAdmin(CreateAccountCommand command)
        {
            var administrators = await _userManager.GetUsersInRoleAsync(AllRoles.Administrator);
            if (administrators.Any())
            {
                throw new Exception("Administrator was already existed");
            }

            var account = Account.CreateNewAccount(command.Email, command.FirstName, command.LastName, true, null);
            var result = await _userManager.CreateAsync(account, command.Password);
            if (!result.Succeeded)
            {
                return Ok(new
                {
                    Result = result
                });
            }

            result = await _userManager.AddToRoleAsync(account, AllRoles.Administrator);
            return Ok(new
            {
                Result = result,
                AccountId = account.Id,
            });
        }

        [HttpPut("account/information")]
        public async Task<IActionResult> UpdateAccountInformation(UpdateInformationCommand command)
        {
            var account = await _userManager.FindByIdAsync(command.AccountId.ToString());
            account.Update(command.FirstName, command.LastName, command.ProfileImageUrl, "", "");
            var result = await _userManager.UpdateAsync(account);
            return Ok(result);
        }

        [HttpPut("account/email")]
        public async Task<IActionResult> ChangeEmailAddress(ChangeEmailAddressCommand command)
        {
            var account = await _userManager.FindByIdAsync(command.AccountId.ToString());
            var token = await _userManager.GenerateChangeEmailTokenAsync(account, command.Email);
            var result = await _userManager.ChangeEmailAsync(account, command.Email, token);
            return Ok(result);
        }

        [HttpPut("account/password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            var account = await _userManager.FindByIdAsync(command.AccountId.ToString());
            account.PasswordHash = _passwordHasher.HashPassword(account, command.Password);
            var result = await _userManager.UpdateAsync(account);
            return Ok(result);
        }

        [HttpPut("account/availability")]
        public async Task<IActionResult> ChangeAvailability(ChangeAvailablityCommand command)
        {
            var account = await _userManager.FindByIdAsync(command.AccountId.ToString());
            account.UpdateStatus(command.IsActive);
            var result = await _userManager.UpdateAsync(account);
            return Ok(result);
        }

        [HttpDelete("account/{accountId}")]
        public async Task<IActionResult> DeleteAccount(Guid accountId)
        {
            var account = await _userManager.FindByIdAsync(accountId.ToString());
            var result = await _userManager.DeleteAsync(account);
            return Ok(result);
        }

        [HttpPut("own/information")]
        public async Task<IActionResult> UpdateOwnAccountInformation(UpdateOwnInformationCommand command)
        {
            var account = await _userManager.FindByIdAsync(CurrentAccountId);
            account.UpdateInfo(command.FirstName, command.LastName, command.ProfileImageUrl, "", "");
            var result = await _userManager.UpdateAsync(account);
            return Ok(result);
        }

        [HttpPut("own/email")]
        public async Task<IActionResult> ChangeOwnEmailAddress(UpdateOwnEmailCommand command)
        {
            var account = await _userManager.FindByIdAsync(CurrentAccountId);
            var isValidPassword = await _userManager.CheckPasswordAsync(account, command.Password);
            if (!isValidPassword)
                return Ok(IdentityResult.Failed(new IdentityError { Description = _localizer["Invalid username and password"] }));
            var token = await _userManager.GenerateChangeEmailTokenAsync(account, command.NewEmailAddress);
            var result = await _userManager.ChangeEmailAsync(account, command.NewEmailAddress, token);
            return Ok(result);
        }

        [HttpPut("own/password")]
        public async Task<IActionResult> ChangeOwnPassword(UpdateOwnPasswordCommand command)
        {
            var account = await _userManager.FindByIdAsync(CurrentAccountId);
            var isValidPassword = await _userManager.CheckPasswordAsync(account, command.CurrentPassword);
            if (!isValidPassword)
                return Ok(IdentityResult.Failed(new IdentityError { Description = _localizer["Invalid username and password"] }));

            account.PasswordHash = _passwordHasher.HashPassword(account, command.NewPassword);
            var result = await _userManager.UpdateAsync(account);
            return Ok(result);
        }
    }
}
