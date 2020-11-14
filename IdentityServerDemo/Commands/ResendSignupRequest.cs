using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Commands
{
    public class ResendSignupRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
