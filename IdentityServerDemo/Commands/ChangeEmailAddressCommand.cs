using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class ChangeEmailAddressCommand
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
