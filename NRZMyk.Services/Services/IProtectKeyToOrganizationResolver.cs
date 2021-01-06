using System.Threading.Tasks;

namespace NRZMyk.Services.Services
{
    public interface IProtectKeyToOrganizationResolver
    {
        Task<string> ResolveOrganization(string protectKey);
    }
}