using System.Collections.Generic;
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
using Tynamix.ObjectFiller;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class ListByOrganizationTests
    {
        private Filler<SentinelEntry> _filler = new Filler<SentinelEntry>();

        [Test]
        public async Task WhenProtectKeyIsPositive_QueriesByProtectKey()
        {
            var sut = CreateSut(out var repository);
            var expectedResult = _filler.Create(2);
            repository.ListAsync(Arg.Is<SentinelEntryFilterSpecification>(specification => specification.ProtectKey == "567"))
                .Returns(Task.FromResult((IReadOnlyList<SentinelEntry>)expectedResult));

            var action = await sut.HandleAsync(567);

            action.Result.Should().BeOfType<OkObjectResult>();
            action.Result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        }

        [Test]
        public async Task WhenProtectKeyIsNegative_QueriesAllEntries()
        {
            var sut = CreateSut(out var repository);
            var expectedResult = _filler.Create(2);
            repository.ListAsync(Arg.Any<AllSentinelEntriesFilterSpecification>())
                .Returns(Task.FromResult((IReadOnlyList<SentinelEntry>)expectedResult));

            var action = await sut.HandleAsync(-1);

            action.Result.Should().BeOfType<OkObjectResult>();
            action.Result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        }


        private static ListByOrganization CreateSut(out IAsyncRepository<SentinelEntry> repository)
        {
            repository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            return new ListByOrganization(repository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}