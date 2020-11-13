using System.Threading.Tasks;

namespace Nmb.Shared.UowInterceptor.Abstracts
{
    public interface IOnBeforeCommit
    {
        Task OnBeforeCommit();
    }
}
