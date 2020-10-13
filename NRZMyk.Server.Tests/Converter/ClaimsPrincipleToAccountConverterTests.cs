using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using NRZMyk.Server.Converter;
using NRZMyk.Services.Data.Entities;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Converter
{
    public class ClaimsPrincipleToAccountConverterTests
    {
        private const ResolutionContext ResolutionContextNotUsed = null;

        [Test]
        public void WhenAllClaimsArePresent_AccountIsFilledWithAllProperties()
        {
            var claims = new Dictionary<string, string>
            {
                {"oid", "dd89e54b-3c19-4db3-a234-3855d7428ef4"},
                {"name", "Jane Doe"},
                {"streetAddress", "Long Road 10029"},
                {"city", "Big City"},
                {"postalCode", "O-773382"},
                {"country", "Germany"},
                {"extension_Role", "8"},
                {"emails", "[\"jane.doe@great-email-provider.com\"]"}
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value))));
            var account = new RemoteAccount();
            var sut = CreateSut();

            sut.Convert(claimsPrincipal, account, ResolutionContextNotUsed);

            account.ObjectId.Should().Be(new Guid("dd89e54b-3c19-4db3-a234-3855d7428ef4"));
            account.DisplayName.Should().Be("Jane Doe");
            account.Country.Should().Be("Germany");
            account.Postalcode.Should().Be("O-773382");
            account.City.Should().Be("Big City");
            account.Street.Should().Be("Long Road 10029");
            account.Email.Should().Be("jane.doe@great-email-provider.com");
        }

        [Test]
        public void WhenSingleEmail_AddressIsParsedCorrectly()
        {
            var claims = new Dictionary<string, string>
            {
                {"emails", "jane.doe@great-email-provider.com"},
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value))));
            var account = new RemoteAccount();
            var sut = CreateSut();

            sut.Convert(claimsPrincipal, account, ResolutionContextNotUsed);

            account.Email.Should().Be("jane.doe@great-email-provider.com");
        }

        [Test]
        public void WhenFullObjectIdentifierType_ObjectIdIsParsedCorrectly()
        {
            var claims = new Dictionary<string, string>
            {
                {"http://schemas.microsoft.com/identity/claims/objectidentifier", "dd89e54b-3c19-4db3-a234-3855d7428ef4"}
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value))));
            var account = new RemoteAccount();
            var sut = CreateSut();

            sut.Convert(claimsPrincipal, account, ResolutionContextNotUsed);

            account.ObjectId.Should().Be("dd89e54b-3c19-4db3-a234-3855d7428ef4");
        }

        [Test]
        public void WhenDestinationIsNull_NewOneGetsCreated()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var sut = CreateSut();

            var account = sut.Convert(claimsPrincipal, null, ResolutionContextNotUsed);

            account.Should().NotBeNull();
            account.ObjectId.Should().Be(Guid.Empty);
            account.DisplayName.Should().BeNull();
            account.Street.Should().BeNull();
            account.Postalcode.Should().BeNull();
            account.Country.Should().BeNull();
            account.Email.Should().BeNull();
        }

        private ClaimsPrincipalToAccountConverter CreateSut()
        {
            return new ClaimsPrincipalToAccountConverter();
        }
    }
}