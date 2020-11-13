using System.Threading.Tasks;

namespace Nmb.Shared.UowInterceptor.Abstracts
{
    public interface IOnDelete<T> where T : class
    {
        Task OnDelete(T entity);
    }
}
