
using IdentityServerDemo.Domain.AccountAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nmb.Shared.Constants;
using Nmb.Shared.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerDemo.Initializations
{
    public class SeedRoleInitialization : IInitializationStage
    {
        private readonly RoleManager<Role> _roleManager;

        public SeedRoleInitialization(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public int Order => 3;
        public async Task ExecuteAsync()
        {
            var currentRoles = await _roleManager.Roles.ToListAsync();
            var allRoles = new List<Role>
            {
                new Role {Name = AllRoles.Administrator},
                new Role {Name = AllRoles.Mobile}
            };
            var newRoles = allRoles.Except(currentRoles).ToList();
            foreach (var item in newRoles)
            {
                await _roleManager.CreateAsync(item);
            }
        }
    }
}
