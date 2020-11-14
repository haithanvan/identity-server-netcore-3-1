using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServerDemo.Commands
{
    public class ChangeAvailablityCommand
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
