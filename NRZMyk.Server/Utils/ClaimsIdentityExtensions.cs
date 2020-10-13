using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NRZMyk.Services.Utils;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Utils
{
    public static class ClaimsIdentityExtensions
    {
        

        public static async Task AddOrganizationClaim(this ClaimsIdentity identity, TokenValidatedContext context)
        {
            var objectId = identity.Claims.ObjectId();
            var accountRepository = context.HttpContext.RequestServices.GetRequiredService<IAsyncRepository<RemoteAccount>>();
            var account = await accountRepository.FirstOrDefaultAsync(new RemoteAccountByObjectIdSpecification(objectId));
            if (account.OrganizationId.HasValue)
            {
                identity.AddClaim(new Claim(ClaimTypes.Organization, account.OrganizationId.ToString()));
                identity.AddClaim(new Claim(System.Security.Claims.ClaimTypes.Role, nameof(Role.User)));
            }
        }
    }
}