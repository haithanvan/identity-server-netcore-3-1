using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class RegisterExternalUserRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
