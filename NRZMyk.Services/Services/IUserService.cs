using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services;

public interface IUserService
{
    Task GetRolesViaGraphApi(IEnumerable<RemoteAccount> remoteAccounts);
    Task UpdateUserRole(string userId, Role role);
}