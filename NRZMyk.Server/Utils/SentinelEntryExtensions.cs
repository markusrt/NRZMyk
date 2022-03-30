using System.Security.Claims;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Server.Utils;

public static class SentinelEntryExtensions
{
    
    /// <summary>
    ///   Checks if the entry exists and if it is accessible by the corresponding user.
    ///   If <paramref name="byPassRole"/> is defined this can be used to force access
    ///   the record if user has this role.
    /// </summary>
    /// <param name="entry">Entry to access, may be <c>null</c></param>
    /// <param name="user">User to check access for</param>
    /// <param name="byPassRole">Role which is allowed access irrespective of other access limitations</param>
    /// <returns></returns>
    public static bool IsNullOrProtected(this SentinelEntry entry, ClaimsPrincipal user, Role? byPassRole=null)
    {
        var organizationId = user.Claims.OrganizationId();
        var userIsInRole = byPassRole.HasValue && user.IsInRole(byPassRole.Value.ToString());
        return entry is null || (entry.ProtectKey != organizationId && !userIsInRole);
    }
}