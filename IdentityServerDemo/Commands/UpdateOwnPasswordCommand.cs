using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class UpdateOwnPasswordCommand
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
