using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class UpdateOwnEmailCommand
    {
        [Required]
        [EmailAddress]
        public string NewEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
