using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class OtherLaboratoryNumbersTests
    {
        private readonly Filler<SentinelEntry> _filler = new();

        [Test]
        public async Task WhenEntriesExist_ReturnsLaboratoryNumbers()
        {
            var sut = CreateSut(out var repository, "567");
            var entries = _filler.Create(3);
            var sequentialNumber = 50;
            foreach (var entry in entries)
            {
                entry.YearlySequentialEntryNumber = sequentialNumber--;
                entry.Year = 2007;
            }
            repository.ListAsync(Arg.Is<SentinelEntryFilterSpecification>(specification => specification.ProtectKey == "567"))
                .Returns(Task.FromResult((IReadOnlyList<SentinelEntry>)entries));

            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<OkObjectResult>();
            var laboratoryNumbers = action.Result.As<OkObjectResult>().Value.As<List<string>>();
            laboratoryNumbers.Should().NotBeNull();
            laboratoryNumbers.Should().ContainInOrder(
                "SN-2007-0048", "SN-2007-0049", "SN-2007-0050");
        }

        [Test]
        public async Task WhenNoEntryFound_ReturnsEmptyList()
        {
            var sut = CreateSut(out var repository, "567");
            var entries = new List<SentinelEntry>();
            repository.ListAsync(Arg.Is<SentinelEntryFilterSpecification>(specification => specification.ProtectKey == "567"))
                .Returns(Task.FromResult((IReadOnlyList<SentinelEntry>)entries));

            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<OkObjectResult>();
            action.Result.As<OkObjectResult>().Value.As<List<string>>().Should().BeEmpty();
        }

        [Test]
        public async Task WhenNoOrganization_AccessDenied()
        {
            var sut = CreateSut(out var repository, "");

            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<ForbidResult>();
            await repository.Received(0).ListAsync(Arg.Any<SentinelEntryFilterSpecification>()).ConfigureAwait(true);
        }

        private static OtherLaboratoryNumbers CreateSut(out ISentinelEntryRepository repository, string organizationId)
        {
            repository = Substitute.For<ISentinelEntryRepository>();
            return new OtherLaboratoryNumbers(repository)
            {
                ControllerContext = new MockControllerContext(organizationId:organizationId)
            };
        }
    }
}