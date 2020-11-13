using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Nmb.Shared.Initialization;
using IdentityServerDemo.Infrastructure;

namespace IdentityServerDemo.Initializations
{
    public class MigrateDatabaseInitialization : IInitializationStage
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;

        public MigrateDatabaseInitialization(
            ApplicationDbContext dbContext,
            PersistedGrantDbContext persistedGrantDbContext,
            ConfigurationDbContext configurationDbContext)
        {
            _dbContext = dbContext;
            _persistedGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
        }

        public int Order => 1;

        public async Task ExecuteAsync()
        {
            await _configurationDbContext.Database.MigrateAsync();
            await _persistedGrantDbContext.Database.MigrateAsync();
            await _dbContext.Database.MigrateAsync();
        }
    }
}
