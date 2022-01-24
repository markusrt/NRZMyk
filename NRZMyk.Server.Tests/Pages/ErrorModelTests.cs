using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NRZMyk.Server.Pages;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Pages
{
    public class ErrorModelTests
    {
        [Test]
        public void Ctor_DoesNotThrow()
        {
            var sut = new ErrorModel(Substitute.For<ILogger<ErrorModel>>());

            sut.Should().NotBeNull();
        }

        [Test]
        public void OnGet_AssignsRequestId()
        {
            var httpContext = new DefaultHttpContext();
            var sut = new ErrorModel(Substitute.For<ILogger<ErrorModel>>())
            {
                PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext { HttpContext = httpContext }
            };

            sut.OnGet();

            sut.RequestId.Should().Be(httpContext.TraceIdentifier);
        }

    }
}