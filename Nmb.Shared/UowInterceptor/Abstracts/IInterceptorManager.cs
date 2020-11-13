using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nmb.Shared.UowInterceptor.Abstracts
{
    public interface IInterceptorManager
    {
        Task Execute(IEnumerable<EntityEntry> changeSet);
        Task BeforeCommit();
        Task AfterCommit();                 
    }
}
