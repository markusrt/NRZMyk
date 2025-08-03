using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Authorization;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class ListPagedTests
    {
        [Test]
        public async Task WhenSuperUserSpecifiesOrganization_AllowsAccess()
        {
            // Arrange
            var user = CreateSuperUser();
            var sut = CreateSut(out var repository, "1", user);
            var request = new ListPagedSentinelEntryRequest 
            { 
                PageSize = 10, 
                PageIndex = 0, 
                OrganizationId = 5 
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            // Act
            var result = await sut.HandleAsync(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            
            // Verify that the specifications were called with the requested organization ID
            await repository.Received(1).CountAsync(Arg.Is<SentinelEntrySearchFilterSpecification>(
                spec => spec.ProtectKey == "5"));
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.ProtectKey == "5"));
        }

        [Test]
        public async Task WhenRegularUserSpecifiesOwnOrganization_AllowsAccess()
        {
            // Arrange
            var user = CreateRegularUser("3");
            var sut = CreateSut(out var repository, "3", user);
            var request = new ListPagedSentinelEntryRequest 
            { 
                PageSize = 10, 
                PageIndex = 0, 
                OrganizationId = 3 
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            // Act
            var result = await sut.HandleAsync(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            
            // Verify that the specifications were called with the user's organization ID
            await repository.Received(1).CountAsync(Arg.Is<SentinelEntrySearchFilterSpecification>(
                spec => spec.ProtectKey == "3"));
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.ProtectKey == "3"));
        }

        [Test]
        public async Task WhenRegularUserSpecifiesDifferentOrganization_ReturnsForbid()
        {
            // Arrange
            var user = CreateRegularUser("3");
            var sut = CreateSut(out var repository, "3", user);
            var request = new ListPagedSentinelEntryRequest 
            { 
                PageSize = 10, 
                PageIndex = 0, 
                OrganizationId = 7  // Different from user's organization (3)
            };

            // Act
            var result = await sut.HandleAsync(request);

            // Assert
            result.Result.Should().BeOfType<ForbidResult>();
            
            // Verify that no repository calls were made
            await repository.DidNotReceive().CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>());
            await repository.DidNotReceive().ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>());
        }

        [Test]
        public async Task WhenNoOrganizationSpecified_UsesUserOrganization()
        {
            // Arrange
            var user = CreateRegularUser("4");
            var sut = CreateSut(out var repository, "4", user);
            var request = new ListPagedSentinelEntryRequest 
            { 
                PageSize = 10, 
                PageIndex = 0 
                // No OrganizationId specified
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            // Act
            var result = await sut.HandleAsync(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            
            // Verify that the specifications were called with the user's organization ID
            await repository.Received(1).CountAsync(Arg.Is<SentinelEntrySearchFilterSpecification>(
                spec => spec.ProtectKey == "4"));
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.ProtectKey == "4"));
        }

        [Test]
        public void WhenPerformingAuthorization_RestrictsToUsersWithinAnOrganization()
        {
            // Arrange
            var type = typeof(ListPaged);

            // Act
            var attribute = type.GetCustomAttribute(typeof(AuthorizeAttribute)) as AuthorizeAttribute;

            // Assert
            attribute.Should().NotBeNull();
            attribute.Policy.Should().Be(Policies.AssignedToOrganization);
            attribute.Roles.Should().Contain(nameof(Role.User));
        }

        private static ClaimsPrincipal CreateSuperUser()
        {
            var user = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(identity.RoleClaimType, nameof(Role.SuperUser)));
            user.AddIdentity(identity);
            return user;
        }

        private static ClaimsPrincipal CreateRegularUser(string organizationId)
        {
            var user = new ClaimsPrincipal();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(identity.RoleClaimType, nameof(Role.User)));
            identity.AddClaim(new Claim(ClaimTypes.Organization, organizationId));
            user.AddIdentity(identity);
            return user;
        }

        private static ListPaged CreateSut(out IAsyncRepository<SentinelEntry> repository, string organizationId = null, ClaimsPrincipal user = null)
        {
            repository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            return new ListPaged(repository)
            {
                ControllerContext = new MockControllerContext(organizationId: organizationId, user: user)
            };
        }
    }
}