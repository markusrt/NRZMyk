using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Mocks.TestUtils;

public class MockControllerContext: ControllerContext
{
    public MockControllerContext(string path = null, string organizationId = null, ClaimsPrincipal user = null)
    {
        HttpContext = new DefaultHttpContext
        {
            Request =
            {
                Host = new HostString("localhost"),
                Scheme = "http",
                Path = path
            },
        };

        var identity = new ClaimsIdentity();
        if (organizationId != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.Organization, organizationId));
        }

        if (user != null)
        {
            user.AddIdentity(identity);
            HttpContext.User = user;
        }
        else
        {
            HttpContext.User = new ClaimsPrincipal(identity);
        }
    }
}