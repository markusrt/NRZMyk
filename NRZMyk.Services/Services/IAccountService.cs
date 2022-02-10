using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface IAccountService
    {
        Task<ICollection<RemoteAccount>> ListAccounts();
        
        Task<ICollection<Organization>> ListOrganizations();
        
        Task<int> AssignToOrganization(ICollection<RemoteAccount> accounts);
    }
}