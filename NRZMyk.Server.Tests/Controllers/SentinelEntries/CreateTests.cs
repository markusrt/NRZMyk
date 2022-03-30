using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Authorization;
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
    public class CreateTests
    {
        [Test]
        public void WhePerformingAuthorization_RestrictsToUsersWithinAnOrganization()
        {
            var type = typeof(Create);

            var attribute = type.GetCustomAttribute(typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();

            attribute.Should().NotBeNull();
            attribute.Policy.Should().Be(Policies.AssignedToOrganization);
            attribute.Roles.Should().Contain(nameof(Role.User));
        }

        [Test]
        public async Task WhenCreatedWithInvalidPredecessorNumber_BadRequest()
        {
            var sut = CreateSut(out var repository, out var mapper, "12");
            var createSentinelEntryRequest = new SentinelEntryRequest() { PredecessorLaboratoryNumber = "SN-3022-0001"};
            var sentinelEntry = new SentinelEntry {Id = 123};

            var action = await sut.HandleAsync(createSentinelEntryRequest).ConfigureAwait(true);
            
            action.Result.Should().BeOfType<BadRequestObjectResult>();
            await repository.Received(0).AddAsync(sentinelEntry).ConfigureAwait(true);
        }

        [Test]
        public async Task WhenCreated_MapsAndStoresToRepository()
        {
            var sut = CreateSut(out var repository, out var mapper, "12");
            var createSentinelEntryRequest = new SentinelEntryRequest();
            var sentinelEntry = new SentinelEntry {Id = 123};
            mapper.Map<SentinelEntry>(createSentinelEntryRequest).Returns(sentinelEntry);
            repository.AddAsync(sentinelEntry).Returns(sentinelEntry);

            var action = await sut.HandleAsync(createSentinelEntryRequest).ConfigureAwait(true);

            await repository.Received(1).AddAsync(sentinelEntry).ConfigureAwait(true);
            sentinelEntry.ProtectKey.Should().Be("12");
            var createdResult = action.Result.Should().BeOfType<CreatedResult>().Subject;
            createdResult.Value.Should().Be(sentinelEntry);
            createdResult.Location.Should().Be("http://localhost/api/sentinel-entries/123");
        }

        [Test]
        public async Task WhenCreatedWithValidPredecessor_StoresRelationToRepository()
        {
            SentinelEntry createdEntry = null;
            var sut = CreateSut(out var repository, out var mapper, "12");
            var createSentinelEntryRequest = new SentinelEntryRequest {PredecessorLaboratoryNumber = "SN-2022-0001"};
            var sentinelEntry = new SentinelEntry {Id = 124};
            var predecessorSentinelEntry = new SentinelEntry {Id = 100};
            mapper.Map<SentinelEntry>(createSentinelEntryRequest).Returns(sentinelEntry);
            repository.When(r => r.AddAsync(Arg.Any<SentinelEntry>())).Do(call => createdEntry = call.Arg<SentinelEntry>());
            repository.AddAsync(sentinelEntry).Returns(sentinelEntry);
            repository.FirstOrDefaultAsync(Arg.Any<SentinelEntryByLaboratoryNumberSpecification>()).Returns(predecessorSentinelEntry);

            var action = await sut.HandleAsync(createSentinelEntryRequest).ConfigureAwait(true);

            await repository.Received(1).FirstOrDefaultAsync(Arg.Is<SentinelEntryByLaboratoryNumberSpecification>(
                s => s.Year == 2022 && s.SequentialNumber == 1 && s.ProtectKey == "12")).ConfigureAwait(true);
            createdEntry.PredecessorEntryId.Should().Be(100);
            var createdResult = action.Result.Should().BeOfType<CreatedResult>().Subject;
        }

        private static Create CreateSut(out ISentinelEntryRepository sentinelEntryRepository, out IMapper map, string organizationId = null)
        {
            sentinelEntryRepository = Substitute.For<ISentinelEntryRepository>();
            map = Substitute.For<IMapper>();
            return new Create(sentinelEntryRepository, map)
            {
                ControllerContext = new MockControllerContext("/api/sentinel-entries", organizationId)
            };
        }
    }
}