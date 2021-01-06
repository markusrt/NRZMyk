using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using Tynamix.ObjectFiller;
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class ExcelExportTests
    {
        [Test]
        public async Task WhenExportCalled_GeneratesExcelFile()
        {
            var sut = CreateSut(out var repository);
            var filler = new Filler<SentinelEntry>();
            repository.ListAsync(Arg.Any<SentinelEntriesIncludingTestsSpecification>())
                .Returns(Task.FromResult((IReadOnlyList<SentinelEntry>)filler.Create(10)));

            var action = await sut.DownloadExcel();

            var fileResult = action.Should().BeOfType<FileContentResult>().Subject;
            fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        private static ExcelExport CreateSut(out IAsyncRepository<SentinelEntry> repository)
        {
            repository = Substitute.For<IAsyncRepository<SentinelEntry>>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            var identity = new ClaimsIdentity();
            httpContext.User = new ClaimsPrincipal(identity);
            return new ExcelExport(repository, Substitute.For<IProtectKeyToOrganizationResolver>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}