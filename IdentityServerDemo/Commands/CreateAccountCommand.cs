using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class CreateAccountCommand
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
