using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NRZMyk.Services.Utils;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Utils
{
    public class ClaimsIdentityExtensionsTests
    {
        [Test]
        public void WhenExtensionFlagClaimIsPresent_RolesAreAdded()
        {
            var claims = new Dictionary<string, string>
            {
                {"extension_Role", "14"}
            };
            var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)));

            identity.AddRolesFromExtensionClaim();

            identity.HasClaim(ClaimTypes.Role, "SuperUser").Should().BeTrue();
            identity.HasClaim(ClaimTypes.Role, "User").Should().BeTrue();
            identity.HasClaim(ClaimTypes.Role, "Admin").Should().BeTrue();
        }

        [Test]
        public void WhenExtensionSingleClaimIsPresent_RoleIsAdded()
        {
            var claims = new Dictionary<string, string>
            {
                {"extension_Role", "8"}
            };
            var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)));

            identity.AddRolesFromExtensionClaim();

            identity.HasClaim(ClaimTypes.Role, "SuperUser").Should().BeTrue();
        }

        [Test]
        public void WhenExtensionSingleClaimIsInvalid_GuestRoleIsAdded()
        {
            var claims = new Dictionary<string, string>
            {
                {"extension_Role", "invalid"}
            };
            var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)));

            identity.AddRolesFromExtensionClaim();

            identity.HasClaim(ClaimTypes.Role, "Guest").Should().BeTrue();
        }
        
        [Test]
        public void WhenExtensionSingleClaimIsMissing_GuestRoleIsAdded()
        {
            var identity = new ClaimsIdentity();

            identity.AddRolesFromExtensionClaim();

            identity.HasClaim(ClaimTypes.Role, "Guest").Should().BeTrue();
        }
    }
}