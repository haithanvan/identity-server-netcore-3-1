using System.Threading.Tasks;

namespace Nmb.Shared.UowInterceptor.Abstracts
{
    public interface IOnAdd<T> where T : class
    {
        Task OnAdd(T entity);
    }
}
