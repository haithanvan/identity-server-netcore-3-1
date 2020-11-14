using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class UpdateOwnInformationCommand
    {
        [Required]
        [MaxLength(200)]
        public string FirstName { get;set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }
        
        public string ProfileImageUrl { get; set; }
    }
}
