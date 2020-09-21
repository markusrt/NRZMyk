using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class GetByIdTests
    {
        [Test]
        public async Task WhenNotFound_Returns404()
        {
            var sut = CreateSut(out var repository);
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 123))
                .Returns(Task.FromResult((SentinelEntry)null));

            var action = await sut.HandleAsync(123);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task WhenFound_ReturnsCorrespondingObject()
        {
            var sut = CreateSut(out var repository);
            var sentinelEntry = new SentinelEntry();
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(567);

            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(sentinelEntry);
        }

        private static GetById CreateSut(out IAsyncRepository<SentinelEntry> repository)
        {
            repository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            return new GetById(repository);
        }
    }
}