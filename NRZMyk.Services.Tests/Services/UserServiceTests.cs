using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Services;

public class UserServiceTests
{
    private const string B2CExtensionAppClientId = "1234-5678";
    private const string RoleCompleteAttributeName = "extension_12345678_Role";

    [Test]
    public void Ctor_ThrowsOnMissingConfig()
    {
        Action createSut = () => new UserService(Substitute.For<IGraphServiceClient>(),
            Options.Create(new AzureAdB2CSettings()), Substitute.For<ILogger<UserService>>());

        createSut.Should().Throw<ArgumentException>();
    }

    [Test]
    public async Task WhenUpdatingRole_SendsUpdateRequestToToGraphClient()
    {
        const string userId = "123";
        User updatedUser = null;
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var userRequest = Substitute.For<IUserRequest>();
        await userRequest.UpdateAsync(Arg.Do<User>(arg => updatedUser = arg)).ConfigureAwait(true);
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        graphServiceClient.Users[userId].Returns(userRequestBuilder);

        await sut.UpdateUserRole(userId, Role.Admin).ConfigureAwait(true);

        updatedUser.Should().NotBeNull();
        updatedUser.AdditionalData[RoleCompleteAttributeName].Should().Be("8");
        logger.Received(1).Log(LogLevel.Information, Arg.Is<string>(
            s => new Regex("Updated role.*Admin.*123").IsMatch(s)));
    }

    [Test]
    public async Task WhenUpdatingFails_LogsError()
    {
        const string userId = "123";
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var exception = new Exception();
        graphServiceClient.Users[userId].Throws(exception);

        await sut.UpdateUserRole(userId, Role.Admin).ConfigureAwait(true);

        logger.Received(1).Log(LogLevel.Error, Arg.Is<string>(
            s => new Regex("Failed to update.*Admin.*123").IsMatch(s)), exception);
    }


    [Test]
    public async Task WhenGettingRolesWithEmptyList_DoesNothing()
    {
        var sut = CreateSut(out _, out _, out _);
        var remoteAccounts = new List<RemoteAccount>();

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        remoteAccounts.Should().BeEmpty();
    }

    [Test]
    public async Task WhenRemoteRequestFailed_LogsErrorAndSetsGuestRole()
    {
        var guid = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid },
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var serviceException = new ServiceException(new Error(), null, HttpStatusCode.BadRequest);
        graphServiceClient.Users[guid.ToString()].Throws(serviceException);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        remoteAccounts.Should().OnlyContain(a => a.Role == Role.Guest);
        logger.Received(1).Log(LogLevel.Error, Arg.Is<string>(s => s.StartsWith(
            $"Failed to query user with ID {guid}")), serviceException);
    }

    [Test]
    public async Task WhenRemoteUserNotFound_LogsWarningAndSetsGuestRole()
    {
        var guid = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid },
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var serviceException = new ServiceException(new Error(), null, HttpStatusCode.NotFound);
        graphServiceClient.Users[guid.ToString()].Throws(serviceException);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        remoteAccounts.Should().OnlyContain(a => a.Role == Role.Guest);
        logger.Received(1).Log(LogLevel.Warning, Arg.Is<string>(s => s.StartsWith(
            $"User with ID {guid} was not found")));
    }

    [Test]
    public async Task WhenGettingRolesForTwoRemoteAccounts_QueriesTwoUsers()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid1 },
            new() { ObjectId = guid2 }
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var userRequest = Substitute.For<IUserRequest>();
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        userRequest.Select($"id,displayName,{RoleCompleteAttributeName}").Returns(userRequest);
        userRequest.GetAsync().Returns(new User
            { AdditionalData = new Dictionary<string, object> { { RoleCompleteAttributeName, "4" } } });
        graphServiceClient.Users[guid1.ToString()].Returns(userRequestBuilder);
        graphServiceClient.Users[guid2.ToString()].Returns(userRequestBuilder);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        userRequest.Received(2).Select(Arg.Any<string>());
        remoteAccounts.Should().OnlyContain(a => a.Role == Role.SuperUser);
    }

    
    [Test]
    public async Task WhenGettingCombinedRoleForRemoteAccounts_AssignsCorrectRoles()
    {
        var guid1 = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid1 },
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var userRequest = Substitute.For<IUserRequest>();
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        userRequest.Select($"id,displayName,{RoleCompleteAttributeName}").Returns(userRequest);
        userRequest.GetAsync().Returns(new User
            { AdditionalData = new Dictionary<string, object> { { RoleCompleteAttributeName, "14" } } });
        graphServiceClient.Users[guid1.ToString()].Returns(userRequestBuilder);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        remoteAccounts.Should().OnlyContain(a => a.Role == (Role.User | Role.SuperUser | Role.Admin));
    }

    [Test]
    public async Task WhenUserDoesNotHaveRoleAssigned_SetsGuestRole()
    {
        var guid = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid },
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var userRequest = Substitute.For<IUserRequest>();
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        userRequest.Select($"id,displayName,{RoleCompleteAttributeName}").Returns(userRequest);
        userRequest.GetAsync().Returns(new User { AdditionalData = null });
        graphServiceClient.Users[guid.ToString()].Returns(userRequestBuilder);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        userRequest.Received(1).Select(Arg.Any<string>());
        remoteAccounts.Should().OnlyContain(a => a.Role == Role.Guest);
    }

    [TestCase("")]
    [TestCase("1234")]
    [TestCase(null)]
    public async Task WhenRolesAttributeIsInvalid_SetsGuestRole(string role)
    {
        var guid = Guid.NewGuid();
        var remoteAccounts = new List<RemoteAccount>
        {
            new() { ObjectId = guid },
        };
        var sut = CreateSut(out var graphServiceClient, out _, out var logger);
        var userRequest = Substitute.For<IUserRequest>();
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        userRequest.Select($"id,displayName,{RoleCompleteAttributeName}").Returns(userRequest);
        userRequest.GetAsync().Returns(new User
            { AdditionalData = new Dictionary<string, object> { { RoleCompleteAttributeName, role } } });
        graphServiceClient.Users[guid.ToString()].Returns(userRequestBuilder);

        await sut.GetRolesViaGraphApi(remoteAccounts).ConfigureAwait(true);

        userRequest.Received(1).Select(Arg.Any<string>());
        remoteAccounts.Should().OnlyContain(a => a.Role == Role.Guest);
    }

    private static UserService CreateSut(out IGraphServiceClient graphServiceClient,
        out IOptions<AzureAdB2CSettings> settings, out MockLogger<UserService> logger)
    {
        settings = Options.Create(new AzureAdB2CSettings());
        settings.Value.AzureAdB2C = new AzureAdB2C()
        {
            B2cExtensionAppClientId = B2CExtensionAppClientId
        };
        graphServiceClient = Substitute.For<IGraphServiceClient>();
        logger = Substitute.For<MockLogger<UserService>>();
        return new UserService(graphServiceClient, settings, logger);
    }
}