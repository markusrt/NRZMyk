using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using NRZMyk.Client;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace NRZMyk.Services.Tests.Services;

public class CustomUserFactoryTests
{
    [Test]
    public void Ctor_DoesNotThrow()
    {
        Action createSut = () => CreateSut(out _, out _, out _);

        createSut.Should().NotThrow();
    }

    [Test]
    public async Task WhenConnectingUserIsNotAuthenticated_UserDoesNotHaveAnyRole()
    {
        var remoteAccount = new RemoteUserAccount
        {
            AdditionalProperties = new Dictionary<string, object>()
        };
        var accessToken = new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken(), "");
        var sut = CreateSut(out var tokenProvider, out _, out _);
        tokenProvider.TokenProvider.RequestAccessToken().Returns(accessToken);

        var claims = await sut.CreateUserAsync(remoteAccount, new RemoteAuthenticationUserOptions()).ConfigureAwait(true);

        claims.IsInRole(Role.Guest.ToString()).Should().BeFalse();
        claims.IsInRole(Role.User.ToString()).Should().BeFalse();
    }

    [Test]
    public async Task WhenAccessTokenRequiresRedirect_UserDoesNotHaveAnyRole()
    {
        var remoteAccount = new RemoteUserAccount
        {
            AdditionalProperties = new Dictionary<string, object>()
        };
        var accessToken = new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, new AccessToken(), "");
        var options = new RemoteAuthenticationUserOptions {AuthenticationType = "Basic"};
        var sut = CreateSut(out var tokenProvider, out _, out _);
        tokenProvider.TokenProvider.RequestAccessToken().Returns(accessToken);

        var claims = await sut.CreateUserAsync(remoteAccount, options).ConfigureAwait(true);

        claims.IsInRole(Role.Guest.ToString()).Should().BeFalse();
        claims.IsInRole(Role.User.ToString()).Should().BeFalse();
    }
    
    [Test]
    public async Task WhenGetConnectFails_UserIsTreatedAsGuest()
    {
        var remoteAccount = new RemoteUserAccount
        {
            AdditionalProperties = new Dictionary<string, object>()
        };
        var accessToken = new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken(), "");
        var options = new RemoteAuthenticationUserOptions {AuthenticationType = "Basic"};
        var sut = CreateSut(out var tokenProvider, out _, out var logger);
        tokenProvider.TokenProvider.RequestAccessToken().Returns(accessToken);

        var claims = await sut.CreateUserAsync(remoteAccount, options).ConfigureAwait(true);

        claims.IsInRole(Role.Guest.ToString()).Should().BeTrue();
        claims.IsInRole(Role.User.ToString()).Should().BeFalse();
        logger.Received(1).LogError(Arg.Any<Exception>(), "Connect API request failed with status code: NotFound");
    }   

    [Test]
    public async Task WhenGetConnectReturnsAccountButDeserializationFails_UserIsTreatedAsGuest()
    {
        var remoteAccount = new RemoteUserAccount
        {
            AdditionalProperties = new Dictionary<string, object>()
        };
        var accessToken = new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken(), "");
        var options = new RemoteAuthenticationUserOptions {AuthenticationType = "Basic"};
        var sut = CreateSut(out var tokenProvider, out var mockHttp, out var logger);
        tokenProvider.TokenProvider.RequestAccessToken().Returns(accessToken);
        mockHttp.When(HttpMethod.Get, "http://localhost/api/user/connect").Respond("application/json", "{ baz }");
        
        var claims = await sut.CreateUserAsync(remoteAccount, options).ConfigureAwait(true);

        claims.IsInRole(Role.Guest.ToString()).Should().BeTrue();
        claims.IsInRole(Role.User.ToString()).Should().BeFalse();
    }
    
    [Test]
    public async Task WhenGetConnectReturnsAccount_UsersRoleIsSet()
    {
        var remoteAccount = new RemoteUserAccount
        {
            AdditionalProperties = new Dictionary<string, object>()
        };
        var accessToken = new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken(), "");
        var options = new RemoteAuthenticationUserOptions {AuthenticationType = "Basic"};
        var sut = CreateSut(out var tokenProvider, out var mockHttp, out _);
        tokenProvider.TokenProvider.RequestAccessToken().Returns(accessToken);
        mockHttp.When(HttpMethod.Get, "http://localhost/api/user/connect")
            .Respond("application/json", "{\"Account\": {\"DisplayName\": \"Jane Doe\"}, \"IsGuest\": false}");
        
        var claims = await sut.CreateUserAsync(remoteAccount, options).ConfigureAwait(true);

        claims.IsInRole(Role.Guest.ToString()).Should().BeFalse();
        claims.IsInRole(Role.User.ToString()).Should().BeTrue();
    }

    private static CustomUserFactory CreateSut(out IAccessTokenProviderAccessor accessTokenProviderAccessor, out MockHttpMessageHandler mockHttp, out ILogger<CustomUserFactory> logger)
    {
        accessTokenProviderAccessor = Substitute.For<IAccessTokenProviderAccessor>();
        mockHttp = new MockHttpMessageHandler();
        logger = Substitute.For<MockLogger<CustomUserFactory>>();
        
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        var clientFactory = Substitute.For<IHttpClientFactory>();
        clientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);
        
        return new CustomUserFactory(accessTokenProviderAccessor, clientFactory, logger);
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