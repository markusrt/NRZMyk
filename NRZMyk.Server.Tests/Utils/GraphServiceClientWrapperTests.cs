using System;
using Azure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Services;
using NUnit.Framework;

namespace NRZMyk.Server.Utils;

public class GraphServiceClientWrapperTests
{
    [Test]
    public void CtorWithMissingConfigSection_DoesThrow()
    {
        Action createSut = () => _ = new GraphServiceClientWrapper(Options.Create(new AzureAdB2CSettings()));

        createSut.Should().Throw<ArgumentException>();
    }

    [Test]
    public void CtorWithValidConfigKeys_DoesNotThrow()
    {
        Action createSut = () => _ = new GraphServiceClientWrapper(Options.Create(new AzureAdB2CSettings {AzureAdB2C = new AzureAdB2C()}));

        createSut.Should().Throw<ArgumentException>();
    }
    
    [Test]
    public void CtorWithValidConfig_DoesNotThrow()
    {
        Action createSut = () => _ = new GraphServiceClientWrapper(Options.Create(new AzureAdB2CSettings {AzureAdB2C = new AzureAdB2C()
        {
            Domain = "foo",
            ClientId = "123",
            ClientSecret = "321"
        }}));

        createSut.Should().NotThrow();
    }

    [Test]
    public void WhenAccessingUsers_ShoulForwardToGraphClient()
    {
        var client = new GraphServiceClientWrapper(Options.Create(new AzureAdB2CSettings {AzureAdB2C = new AzureAdB2C()
        {
            Domain = "foo",
            ClientId = "123",
            ClientSecret = "321"
        }}));

        client.Users.Should().NotBeNull();
    }
}