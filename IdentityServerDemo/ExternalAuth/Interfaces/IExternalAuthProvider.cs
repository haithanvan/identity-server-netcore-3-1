using System.Threading.Tasks;

namespace IdentityServerDemo.ExternalAuth.Interfaces
{
    public interface IExternalAuthProvider
    {
        Task<TokenInfo> GetInfoByToken(string token);
    }
}
