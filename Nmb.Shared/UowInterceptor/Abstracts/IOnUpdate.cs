using System.Threading.Tasks;

namespace Nmb.Shared.UowInterceptor.Abstracts
{
    public interface IOnUpdate<T> where T : class
    {
        Task OnUpdate(T entity);
    }
}
