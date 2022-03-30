using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Authorization;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class DeleteTests
    {
        [Test]
        public void WhePerformingAuthorization_RestrictsToUsersWithinAnOrganization()
        {
            var type = typeof(Delete);

            var attribute = type.GetCustomAttribute(typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();

            attribute.Should().NotBeNull();
            attribute.Policy.Should().Be(Policies.AssignedToOrganization);
            attribute.Roles.Should().Contain(nameof(Role.User));
        }

        [Test]
        public async Task WhenNotFound_Returns404()
        {
            var sut = CreateSut(out var repository, out _, "12");
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(123).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task WhenFoundWithinAnotherOrganization_Returns404()
        {
            var sut = CreateSut(out var repository, out _, "12");
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "24"

            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
            await repository.Received(0).DeleteAsync(Arg.Any<SentinelEntry>()).ConfigureAwait(true);
        }

        [Test]
        public async Task WhenFound_DeletesCorrespondingObjects()
        {
            var sut = CreateSut(out var repository, out var sensitivityTestRepository, "12");
            var sensitivityTest1 = new AntimicrobialSensitivityTest();
            var sensitivityTest2 = new AntimicrobialSensitivityTest();
            var sentinelEntry = new SentinelEntry
            {
                Id = 567,
                ProtectKey = "12",
                AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>
                {
                    sensitivityTest1, sensitivityTest2
                }
            };

            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567).ConfigureAwait(true);

            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest1).ConfigureAwait(true);
            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest2).ConfigureAwait(true);
            await repository.Received(1).DeleteAsync(sentinelEntry).ConfigureAwait(true);
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(567);
        }

        [Test]
        public async Task WhenFoundButAlreadyArchived_DeniesAccess()
        {
            var sut = CreateSut(out var repository, out _, "12");
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "12",
                CryoDate = new DateTime(2010, 10, 10),
                AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>()
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567).ConfigureAwait(true);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>()).ConfigureAwait(true);
        }


        private static Delete CreateSut(out IAsyncRepository<SentinelEntry> sentinelEntryRepository,
            out IAsyncRepository<AntimicrobialSensitivityTest> sensitivityTestRepository, string organizationId = null)
        {
            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            sensitivityTestRepository = Substitute.For<IAsyncRepository<AntimicrobialSensitivityTest>>();

            return new Delete(sentinelEntryRepository, sensitivityTestRepository)
            {
                ControllerContext = new MockControllerContext(organizationId:organizationId)
            };
        }
    }
}