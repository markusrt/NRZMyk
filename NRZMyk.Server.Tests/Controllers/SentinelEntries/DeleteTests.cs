using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class DeleteTests
    {
        [Test]
        public async Task WhenNoOrganizationAssigned_DeniesAccess()
        {
            var sut = CreateSut(out var repository, out _);
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(123);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>());
        }

        [Test]
        public async Task WhenNotFound_Returns404()
        {
            var sut = CreateSut(out var repository, out _, "12");
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(123);

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

            var action = await sut.HandleAsync(567);

            action.Result.Should().BeOfType<NotFoundResult>();
            await repository.Received(0).DeleteAsync(Arg.Any<SentinelEntry>());
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

            var action = await sut.HandleAsync(567);

            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest1);
            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest2);
            await repository.Received(1).DeleteAsync(sentinelEntry);
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

            var action = await sut.HandleAsync(567);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>());
        }


        private static Delete CreateSut(out IAsyncRepository<SentinelEntry> sentinelEntryRepository,
            out IAsyncRepository<AntimicrobialSensitivityTest> sensitivityTestRepository, string organizationId = null)
        {
            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            sensitivityTestRepository = Substitute.For<IAsyncRepository<AntimicrobialSensitivityTest>>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            var identity = new ClaimsIdentity();
            if (organizationId != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Organization, organizationId));
            }
            httpContext.User = new ClaimsPrincipal(identity);
            return new Delete(sentinelEntryRepository, sensitivityTestRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}