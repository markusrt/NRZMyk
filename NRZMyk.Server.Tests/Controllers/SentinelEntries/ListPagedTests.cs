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

            var result = await sut.HandleAsync(request);

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

            var result = await sut.HandleAsync(request);

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
            var user = CreateRegularUser("3");
            var sut = CreateSut(out var repository, "3", user);
            var request = new ListPagedSentinelEntryRequest 
            { 
                PageSize = 10, 
                PageIndex = 0, 
                OrganizationId = 7  // Different from user's organization (3)
            };

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<ForbidResult>();
            
            // Verify that no repository calls were made
            await repository.DidNotReceive().CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>());
            await repository.DidNotReceive().ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>());
        }

        [Test]
        public async Task WhenNoOrganizationSpecified_UsesUserOrganization()
        {
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

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<OkObjectResult>();
            
            // Verify that the specifications were called with the user's organization ID
            await repository.Received(1).CountAsync(Arg.Is<SentinelEntrySearchFilterSpecification>(
                spec => spec.ProtectKey == "4"));
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.ProtectKey == "4"));
        }

        [Test]
        public async Task WhenSuperUserDoesNotSpecifyOrganization_UsesAllOrganizations()
        {
            var user = CreateSuperUser();
            var sut = CreateSut(out var repository, "1", user);
            var request = new ListPagedSentinelEntryRequest
            {
                PageSize = 10,
                PageIndex = 0
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<OkObjectResult>();

            await repository.Received(1).CountAsync(Arg.Is<SentinelEntrySearchFilterSpecification>(
                spec => spec.ProtectKey == null));
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.ProtectKey == null));
        }

        [Test]
        public async Task WhenPageSizeIsZero_AppliesDefaultPageSizeAndDoesNotThrow()
        {
            var user = CreateRegularUser("4");
            var sut = CreateSut(out var repository, "4", user);
            var request = new ListPagedSentinelEntryRequest
            {
                // PageSize and PageIndex left at default int (0)
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(50));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<OkObjectResult>();
            var response = (result.Result as OkObjectResult)!.Value as ListPagedSentinelEntryResponse;
            response.Should().NotBeNull();
            // 50 items at default page size (25) should produce 2 pages
            response!.PageCount.Should().Be(2);
            response.TotalCount.Should().Be(50);
        }

        [Test]
        public void WhenPageSizeIsNotProvided_RequestUsesDefaultPageSize()
        {
            var request = new ListPagedSentinelEntryRequest();

            request.PageSize.Should().Be(ListPagedSentinelEntryRequest.DefaultPageSize);
            request.PageIndex.Should().Be(0);
        }

        [Test]
        public async Task WhenPageSizeExceedsMax_ClampsToMaxPageSize()
        {
            var user = CreateRegularUser("4");
            var sut = CreateSut(out var repository, "4", user);
            var request = new ListPagedSentinelEntryRequest
            {
                PageSize = ListPagedSentinelEntryRequest.MaxPageSize + 5000,
                PageIndex = 0
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<OkObjectResult>();
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.Take == ListPagedSentinelEntryRequest.MaxPageSize));
        }

        [Test]
        public async Task WhenPageIndexIsNegative_ClampsToZero()
        {
            var user = CreateRegularUser("4");
            var sut = CreateSut(out var repository, "4", user);
            var request = new ListPagedSentinelEntryRequest
            {
                PageSize = 10,
                PageIndex = -3
            };

            repository.CountAsync(Arg.Any<SentinelEntrySearchFilterSpecification>())
                .Returns(Task.FromResult(0));
            repository.ListAsync(Arg.Any<SentinelEntrySearchPaginatedSpecification>())
                .Returns(Task.FromResult(new List<SentinelEntry>() as IReadOnlyList<SentinelEntry>));

            var result = await sut.HandleAsync(request);

            result.Result.Should().BeOfType<OkObjectResult>();
            await repository.Received(1).ListAsync(Arg.Is<SentinelEntrySearchPaginatedSpecification>(
                spec => spec.Skip == 0));
        }

        [Test]
        public void WhenPerformingAuthorization_RestrictsToUsersWithinAnOrganization()
        {
            var type = typeof(ListPaged);

            var attribute = type.GetCustomAttribute(typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();

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
