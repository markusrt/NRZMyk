using System;
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
        await userRequest.UpdateAsync(Arg.Do<User>(arg => updatedUser = arg));
        var userRequestBuilder = Substitute.For<IUserRequestBuilder>();
        userRequestBuilder.Request().Returns(userRequest);
        graphServiceClient.Users[userId].Returns(userRequestBuilder);

        await sut.UpdateUserRole(userId, Role.Admin);

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

        await sut.UpdateUserRole(userId, Role.Admin);

        logger.Received(1).Log(LogLevel.Error, Arg.Is<string>(
            s => new Regex("Failed to update.*Admin.*123").IsMatch(s)), exception);
    }

    private static UserService CreateSut(out IGraphServiceClient graphServiceClient, out IOptions<AzureAdB2CSettings> settings, out MockLogger<UserService> logger)
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