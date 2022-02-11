using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class UpdateTests
    {
        [Test]
        public async Task WhenNoOrganizationAssigned_DeniesAccess()
        {
            var sut = CreateSut(out var repository, out _, out _);
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(new SentinelEntryRequest {Id = 123}).ConfigureAwait(true);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>()).ConfigureAwait(true);
        }

        [Test]
        public async Task WhenNotFound_Returns404()
        {
            var sut = CreateSut(out var repository, out _, out _, "12");
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(new SentinelEntryRequest {Id = 123}).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task WhenFoundWithinAnotherOrganization_Returns404()
        {
            var sut = CreateSut(out var repository, out var sensitivityTestRepository, out var mapper, "12");
            var updateSentinelEntry = new SentinelEntryRequest {Id = 567};
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "24"

            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(updateSentinelEntry).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>()).ConfigureAwait(true);
            mapper.Received(0).Map(updateSentinelEntry, sentinelEntry);
        }

        [Test]
        public async Task WhenFound_ReturnsCorrespondingObject()
        {
            var sut = CreateSut(out var repository, out var sensitivityTestRepository, out var mapper, "12");
            var updateSentinelEntry = new SentinelEntryRequest {Id = 567};
            var sensitivityTest1 = new AntimicrobialSensitivityTest();
            var sensitivityTest2 = new AntimicrobialSensitivityTest();
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "12",
                AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>
                {
                    sensitivityTest1, sensitivityTest2
                }
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(updateSentinelEntry).ConfigureAwait(true);

            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest1).ConfigureAwait(true);
            await sensitivityTestRepository.Received(1).DeleteAsync(sensitivityTest2).ConfigureAwait(true);
            mapper.Received(1).Map(updateSentinelEntry, sentinelEntry);
            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(sentinelEntry);
        }

        [Test]
        public async Task WhenFoundButAlreadyArchived_DeniesAccess()
        {
            var sut = CreateSut(out var repository, out _, out _, "12");
            var updateSentinelEntry = new SentinelEntryRequest { Id = 567 };
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "12",
                CryoDate = new DateTime(2010,10,10),
                AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTest>()
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(updateSentinelEntry).ConfigureAwait(true);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).UpdateAsync(Arg.Any<SentinelEntry>()).ConfigureAwait(true);
        }


        private static Update CreateSut(out IAsyncRepository<SentinelEntry> sentinelEntryRepository,
            out IAsyncRepository<AntimicrobialSensitivityTest> sensitivityTestRepository, out IMapper map,
            string organizationId = null)
        {
            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            sensitivityTestRepository = Substitute.For<IAsyncRepository<AntimicrobialSensitivityTest>>();
            map = Substitute.For<IMapper>();

            return new Update(sentinelEntryRepository, sensitivityTestRepository, map)
            {
                ControllerContext = new MockControllerContext(organizationId: organizationId)
            };
        }
    }
}