using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class ChangePasswordCommand
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
