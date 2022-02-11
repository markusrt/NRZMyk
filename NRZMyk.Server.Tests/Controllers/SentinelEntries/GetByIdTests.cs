﻿using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
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
    public class GetByIdTests
    {
        [Test]
        public async Task WhenNotFound_Returns404()
        {
            var sut = CreateSut(out var repository, "12");
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(123).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task WhenNotFoundWithCorrespondingProtectKey_Returns404()
        {
            var sut = CreateSut(out var repository, "12");
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "24"
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(123).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task WhenNoOrganization_AccessDenied()
        {
            var sut = CreateSut(out var repository);
            var sentinelEntry = new SentinelEntry();
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567).ConfigureAwait(true);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).FirstOrDefaultAsync(Arg.Any<SentinelEntryIncludingTestsSpecification>()).ConfigureAwait(true);
        }

        [Test]
        public async Task WhenFound_ReturnsCorrespondingObject()
        {
            var sut = CreateSut(out var repository, "12");
            var sentinelEntry = new SentinelEntry
            {
                ProtectKey = "12"
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567).ConfigureAwait(true);

            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(sentinelEntry);
        }

        private static GetById CreateSut(out IAsyncRepository<SentinelEntry> repository, string organizationId = null)
        {
            repository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            return new GetById(repository)
            {
                ControllerContext = new MockControllerContext(organizationId:organizationId)
            };
        }
    }
}