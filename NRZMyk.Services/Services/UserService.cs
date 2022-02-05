using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Services;

public class UserService : IUserService
{
    private readonly IGraphServiceClient _graphClient;
    private readonly ILogger<UserService> _logger;
    private readonly string _extensionAppClientId;

    private const string RoleAttribute = "Role";

    private string RoleAttributeName { get; set; }

    public UserService(IGraphServiceClient graphClient, IOptions<AzureAdB2CSettings> config, ILogger<UserService> logger)
    {
        if (string.IsNullOrWhiteSpace(config?.Value?.AzureAdB2C?.B2cExtensionAppClientId))
        {
            throw new ArgumentException(
                "Missing extension client id configuration: " +
                $"{nameof(AzureAdB2CSettings.AzureAdB2C)} -> {nameof(AzureAdB2CSettings.AzureAdB2C.B2cExtensionAppClientId)}.",
                nameof(_extensionAppClientId));
        }

        _graphClient = graphClient;
        _logger = logger;
        _extensionAppClientId = config.Value.AzureAdB2C.B2cExtensionAppClientId;

        RoleAttributeName = $"extension_{_extensionAppClientId.Replace("-", "")}_{RoleAttribute}";
    }

    public async Task GetRolesViaGraphApi(IEnumerable<RemoteAccount> remoteAccounts)
    {
        foreach (var remoteAccount in remoteAccounts)
        {
            await GetRoleViaGraphApi(remoteAccount);
        }
    }

    private async Task GetRoleViaGraphApi(RemoteAccount remoteAccount)
    {
        var role = Role.Guest;

        try
        {
            var user = await _graphClient.Users[remoteAccount.ObjectId.ToString()]
                .Request()
                .Select($"id,displayName,{RoleAttributeName}")
                .GetAsync();

            role = TryToGetRoleFromCustomAttribute(user);
        }
        catch (ServiceException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("User with ID {remoteAccount} was not found in AADB2C, assigning guest role",
                    remoteAccount.ObjectId);
            }
            else
            {
                _logger.LogError(e, "Failed to query user with ID {remoteAccount} from AADB2C, assigning guest role",
                    remoteAccount.ObjectId);
            }
        }

        remoteAccount.Role = role;
    }

    private Role TryToGetRoleFromCustomAttribute(User user)
    {
        if (user.AdditionalData?[RoleAttributeName] == null)
        {
            return Role.Guest;
        }
        
        var roleString = user.AdditionalData[RoleAttributeName].ToString();
        var parseSuccess = Enum.TryParse<Role>(roleString, out var role);
        if (parseSuccess && Enum.IsDefined(role))
        {
            return role;
        }

        return Role.Guest;
    }

    public async Task UpdateUserRole(string userId, Role role)
    {
        IDictionary<string, object> extensionInstance = new Dictionary<string, object>
        {
            { RoleAttributeName, ((int)role).ToString() }
        };

        var user = new User
        {
            AdditionalData = extensionInstance
        };


        try
        {
            await _graphClient.Users[userId].Request().UpdateAsync(user);
            _logger.LogInformation("Updated role to '{role}' for user with object ID '{userId}'",
                role, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, 
                "Failed to update role to '{role}' for user with object ID '{userId}'",
                role, userId);
        }
    }
}