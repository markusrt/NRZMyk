using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class CryoRemarkUpdateTests
    {
        [TestCase("Updated remark")]
        [TestCase("")]
        [TestCase(null)]
        public async Task WhenFound_UpdatesOnlyRemark(string expectedRemark)
        {
            var sut = CreateSut(out var repository);
            
            var cryoRemarkUpdateRequest = new CryoRemarkUpdateRequest {Id = 567, CryoRemark = expectedRemark};
            var originalCryoDate = new DateTime(2020, 5, 4);
            var sentinelEntry = new SentinelEntry
            {
                AgeGroup = AgeGroup.EightySixToNinety,
                CryoBoxNumber = 10,
                CryoBoxSlot = 56,
                CryoDate = originalCryoDate,
                CryoRemark = "Original remark"
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(cryoRemarkUpdateRequest).ConfigureAwait(true);

            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var storedEntry = okResult.Value as SentinelEntry;
            storedEntry.CryoRemark.Should().Be(expectedRemark);
            // All other fields should remain unchanged
            storedEntry.CryoDate.Should().Be(originalCryoDate);
            storedEntry.CryoBoxNumber.Should().Be(10);
            storedEntry.CryoBoxSlot.Should().Be(56);
        }

        [Test]
        public async Task WhenNotFound_ReturnsNotFound()
        {
            var sut = CreateSut(out var repository);
            
            var cryoRemarkUpdateRequest = new CryoRemarkUpdateRequest {Id = 999, CryoRemark = "Some remark"};
            repository.FirstOrDefaultAsync(Arg.Any<SentinelEntryIncludingTestsSpecification>())
                .Returns(Task.FromResult<SentinelEntry>(null));

            var action = await sut.HandleAsync(cryoRemarkUpdateRequest).ConfigureAwait(true);

            action.Result.Should().BeOfType<NotFoundResult>();
        }

        private static CryoRemarkUpdate CreateSut(out IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();

            return new CryoRemarkUpdate(sentinelEntryRepository, mapper)
            {
                ControllerContext = new MockControllerContext()
            };
        }
    }
}