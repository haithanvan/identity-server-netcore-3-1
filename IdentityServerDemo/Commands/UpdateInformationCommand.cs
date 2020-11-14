using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class UpdateInformationCommand
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [MaxLength(200)]
        public string FirstName { get;set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }
        
        public string ProfileImageUrl { get; set; }
    }
}
