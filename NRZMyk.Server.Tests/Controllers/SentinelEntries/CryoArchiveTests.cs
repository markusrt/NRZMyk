using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class CryoArchiveTests
    {
        [TestCase("2010-10-10", "A remark")]
        [TestCase(null, "")]
        [TestCase("2020-05-04", null)]
        public async Task WhenFound_UpdatesOnlyDateAndRemark(string dateString, string expectedRemark)
        {
            var expectedDate = dateString != null ? (DateTime?)DateTime.Parse(dateString) : null;
            var sut = CreateSut(out var repository);
            
            var cryoArchiveRequest = new CryoArchiveRequest {Id = 567, CryoDate = expectedDate, CryoRemark = expectedRemark};
            var sentinelEntry = new SentinelEntry
            {
                AgeGroup = AgeGroup.EightySixToNinety,
                CryoBoxNumber = 10,
                CryoBoxSlot = 56
            };
            repository.FirstOrDefaultAsync(Arg.Is<SentinelEntryIncludingTestsSpecification>(specification => specification.Id == 567))
                .Returns(Task.FromResult(sentinelEntry));

            var action = await sut.HandleAsync(cryoArchiveRequest);

            var okResult = action.Result.Should().BeOfType<OkObjectResult>().Subject;
            var storedEntry = okResult.Value as SentinelEntry;
            storedEntry.CryoDate.Should().Be(expectedDate);
            storedEntry.CryoRemark.Should().Be(expectedRemark);
            storedEntry.CryoBoxNumber.Should().Be(10);
            storedEntry.CryoBoxSlot.Should().Be(56);
        }

        private static CryoArchive CreateSut(out IAsyncRepository<SentinelEntry> sentinelEntryRepository)
        {
            var myProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            sentinelEntryRepository = Substitute.For<IAsyncRepository<SentinelEntry>>();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            return new CryoArchive(sentinelEntryRepository, mapper)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}