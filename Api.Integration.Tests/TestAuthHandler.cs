using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace Api.Integration.Tests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    internal const string AuthenticationScheme = "Test";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(NRZMyk.Services.Models.ClaimTypes.Organization, "1") };
        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        identity.AddClaim(new Claim(identity.RoleClaimType, Role.SuperUser.ToString()));
        identity.AddClaim(new Claim(identity.RoleClaimType, Role.User.ToString()));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}