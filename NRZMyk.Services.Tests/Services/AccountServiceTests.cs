using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Services;

public class AccountServiceTests
{
    [Test]
    public void Ctor_DoesNotThrow()
    {
        Action createSut = () => CreateSut(out _);

        createSut.Should().NotThrow();
    }
    
    [Test]
    public async Task WhenListAccounts_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.ListAccounts();

        await httpClient.Received(1).Get<ICollection<RemoteAccount>>(
            "api/users", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenListOrganizations_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        
        await sut.ListOrganizations();

        await httpClient.Received(1).Get<ICollection<Organization>>(
            "api/organizations", default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenAssignToOrganization_CallsCorrectUri()
    {
        var sut = CreateSut(out var httpClient);
        var accounts = new List<RemoteAccount>();
        
        await sut.AssignToOrganization(accounts);

        await httpClient.Received(1).Post<ICollection<RemoteAccount>, int>(
            "api/users/assign-organization", accounts, default, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    private static AccountService CreateSut(out IHttpClient httpClient)
    {
        httpClient = Substitute.For<IHttpClient>();
        return new AccountService(httpClient);
    }
}