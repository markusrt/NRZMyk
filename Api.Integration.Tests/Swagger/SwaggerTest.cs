using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Api.Integration.Tests.Swagger
{
    public class SwaggerTests
    {
        [Test]
        public async Task WhenAccessingIndex_ReturnsPage()
        {
            var client = TestcontainerDbClientFactory.CreateClient();

            var response = await client.GetAsync("swagger/index.html").ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
