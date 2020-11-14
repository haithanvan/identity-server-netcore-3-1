using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class ConfirmSignupCommand
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6), MaxLength(6)]
        public string Otp { get; set; }
    }
}
