using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace NRZMyk.Services.Tests.Services;

public class LoggingJsonHttpClientTests
{
    [Test]
    public void Ctor_DoesNotThrow()
    {
        Action createSut = () => CreateSut(out var _, out var _);

        createSut.Should().NotThrow();
    }

    [Test]
    public async Task WhenGetSucceeds_ResponseIsDeserialized()
    {
        var sut = CreateSut(out var mockHttp, out var logger);

        mockHttp.When(HttpMethod.Get, "http://localhost/api/products/42")
            .Respond("application/json", "{\"Name\":\"Car\"}");

        var product = await sut.Get<Product>("/api/products/42");

        product.Name.Should().Be("Car");
    }
    
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public async Task WhenGetFails_ErrorIsLogged(HttpStatusCode statusCode)
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        NoErrorOnHttpMethods(mockHttp, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete);
        mockHttp.When(HttpMethod.Get, "*").Respond(statusCode);

        var getAction = async () => await sut.Get<Product>("/api/products/42");

        getAction.Should().Throw<Exception>();
        logger.Received(1).LogError(
            $"GET Product on http://localhost/api/products/42 failed with status {statusCode} during {nameof(WhenGetFails_ErrorIsLogged)}",
            Arg.Any<Exception>());
    }

    [Test]
    public async Task WhenGetBase64Succeeds_ResponseIsDeserialized()
    {
        var sut = CreateSut(out var mockHttp, out var logger);

        mockHttp.When(HttpMethod.Get, "http://localhost/api/products/42/download")
            .Respond("application/json", "{\"Foo\":\"Bar\"}");

        var base64 = await sut.GetBytesAsBase64("/api/products/42/download");

        base64.Should().Be("eyJGb28iOiJCYXIifQ==");
    }

    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public async Task WhenGetBase64Fails_ErrorIsLoggedAndEmptyStringReturned(HttpStatusCode statusCode)
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        NoErrorOnHttpMethods(mockHttp, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete);
        mockHttp.When(HttpMethod.Get, "*").Respond(statusCode);

        var base64 = await sut.GetBytesAsBase64("/api/products/42/download");

        logger.Received(1).LogError(
            $"GET Base64 on http://localhost/api/products/42/download failed with status ? during {nameof(WhenGetBase64Fails_ErrorIsLoggedAndEmptyStringReturned)}",
            Arg.Any<Exception>());
    }

    [Test]
    public async Task WhenPostSucceeds_ResponseIsDeserialized()
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        var request = new Product { Name = "Car" };

        mockHttp.When(HttpMethod.Post, "http://localhost/api/products")
            .WithContent("{\"Name\":\"Car\"}")
            .Respond("application/json", "{\"Id\":\"42\"}");

        var createdEntry = await sut.Post<Product, IdResponse>("/api/products", request);

        createdEntry.Id.Should().Be(42);
    }

    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public void WhenPostFails_ErrorIsLogged(HttpStatusCode statusCode)
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        NoErrorOnHttpMethods(mockHttp, HttpMethod.Get, HttpMethod.Put, HttpMethod.Delete);
        mockHttp.When(HttpMethod.Post, "*").Respond(statusCode);

        var postAction = async () => await sut.Post<Product, IdResponse>("/api/products", new Product());

        postAction.Should().Throw<Exception>();
        logger.Received(1).LogError(
            $"POST Product on http://localhost/api/products failed with status {statusCode} during {nameof(WhenPostFails_ErrorIsLogged)}",
            Arg.Any<Exception>());
    }

    private void NoErrorOnHttpMethods(MockHttpMessageHandler mockHttp, params HttpMethod[] methods)
    {
        foreach (var method in methods)
        {
            mockHttp.When(method, "*").Respond("application/json", "{}");
        }
    }

    [Test]
    public async Task WhenPutSucceeds_ResponseIsDeserialized()
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        var request = new Product { Name = "Car" };

        mockHttp.When(HttpMethod.Put, "http://localhost/api/products")
            .WithContent("{\"Name\":\"Car\"}")
            .Respond("application/json", "{\"Id\":\"42\"}");

        var createdEntry = await sut.Put<Product, IdResponse>("/api/products", request);

        createdEntry.Id.Should().Be(42);
    }
    
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public void WhenPutFails_ErrorIsLogged(HttpStatusCode statusCode)
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        NoErrorOnHttpMethods(mockHttp, HttpMethod.Get, HttpMethod.Post, HttpMethod.Delete);
        mockHttp.When(HttpMethod.Put, "*").Respond(statusCode);

        var postAction = async () => await sut.Put<Product, IdResponse>("/api/products", new Product());

        postAction.Should().Throw<Exception>();
        logger.Received(1).LogError(
            $"PUT Product on http://localhost/api/products failed with status {statusCode} during {nameof(WhenPutFails_ErrorIsLogged)}",
            Arg.Any<Exception>());
    }

    [Test]
    public void WhenDeleteSucceeds_DoesNotThrow()
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        mockHttp.When(HttpMethod.Delete, "http://localhost/api/products/42")
            .Respond(HttpStatusCode.OK);

        sut.Invoking(async s => await s.Delete<Product>("/api/products/42")).Should().NotThrow();
    }
    
    [TestCase(HttpStatusCode.Unauthorized)]
    [TestCase(HttpStatusCode.NotFound)]
    [TestCase(HttpStatusCode.BadRequest)]
    public async Task WhenDeleteFails_ErrorIsLogged(HttpStatusCode statusCode)
    {
        var sut = CreateSut(out var mockHttp, out var logger);
        NoErrorOnHttpMethods(mockHttp, HttpMethod.Post, HttpMethod.Put, HttpMethod.Get);
        mockHttp.When(HttpMethod.Delete, "*").Respond(statusCode);

        var deleteAction = async () => await sut.Delete<Product>("/api/products/42");

        deleteAction.Should().Throw<Exception>();
        logger.Received(1).LogError(
            $"DELETE Product on http://localhost/api/products/42 failed with status {statusCode} during {nameof(WhenDeleteFails_ErrorIsLogged)}",
            Arg.Any<Exception>());
    }

    private static LoggingJsonHttpClient CreateSut(out MockHttpMessageHandler mockHttp, out ILogger<LoggingJsonHttpClient> logger)
    {
        mockHttp = new MockHttpMessageHandler();
        logger = Substitute.For<MockLogger<LoggingJsonHttpClient>>();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        return new LoggingJsonHttpClient(httpClient, logger);
    }

    private class Product
    {
        public string Name { get; set; }
    }

    private class IdResponse
    {
        public int Id { get; set; }
    }
}