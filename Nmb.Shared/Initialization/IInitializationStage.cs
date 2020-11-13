using System.Threading.Tasks;

namespace Nmb.Shared.Initialization
{
    public interface IInitializationStage
    {
        int Order { get; }
        Task ExecuteAsync();
    }
}