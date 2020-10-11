using System;
using System.Linq;
using System.Security.Claims;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Utils
{
    public static class ClaimsIdentityExtensions
    {
        public static void AddRolesFromExtensionClaim(this ClaimsIdentity identity)
        {
            var roles = identity.Claims.Where(c => c.Type == "extension_Role").ToList();
            var singleRole = roles.FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(singleRole) && Enum.TryParse<Role>(singleRole, out var role))
            {
                if (role.HasFlag(Role.User))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.User)));
                }

                if (role.HasFlag(Role.Admin))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.Admin)));
                }

                if (role.HasFlag(Role.SuperUser))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.SuperUser)));
                }
            }

            if (!identity.HasClaim(c  => c.Type == ClaimTypes.Role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.Guest)));
            }
        }
    }
}