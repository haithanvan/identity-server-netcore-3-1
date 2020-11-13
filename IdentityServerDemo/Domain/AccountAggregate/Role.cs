using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Domain.AccountAggregate
{
    public class Role : IdentityRole<string>
    {
        public string Description { get; set; }
    }
}
