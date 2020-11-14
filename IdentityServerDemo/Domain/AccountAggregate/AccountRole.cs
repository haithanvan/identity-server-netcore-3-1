using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Domain.AccountAggregate
{
    public class AccountRole : IdentityUserRole<Guid>
    {
        public virtual Role Role { get; set; }
    }
}
