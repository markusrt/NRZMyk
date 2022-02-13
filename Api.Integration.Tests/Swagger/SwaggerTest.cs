using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PublicApiIntegrationTests;

namespace Api.Integration.Tests.Swagger
{
    public class SwaggerTests
    {
        [Test]
        public async Task WhenAccessingIndex_ReturnsPage()
        {
            var client = ClientFactory.CreateClient();

            var response = await client.GetAsync("swagger/index.html").ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
