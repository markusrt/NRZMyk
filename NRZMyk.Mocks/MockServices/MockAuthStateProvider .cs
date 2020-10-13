using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using NRZMyk.Services.Models;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace NRZMyk.Mocks.MockServices
{
    public class MockAuthStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Mrs Mock"),
            }, "Fake authentication type");

            var user = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.Guest)));
            identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.User)));
            identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.SuperUser)));
            identity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.Admin)));
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}