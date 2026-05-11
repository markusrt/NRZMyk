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

    private const string RoleAttribute = "Role";

    private string RoleAttributeName { get; set; }

    public UserService(IGraphServiceClient graphClient, IOptions<AzureAdB2CSettings> config, ILogger<UserService> logger)
    {
        if (string.IsNullOrWhiteSpace(config?.Value?.AzureAdB2C?.B2cExtensionAppClientId))
        {
            throw new ArgumentException(
                "Missing extension client id configuration: " +
                $"{nameof(AzureAdB2CSettings.AzureAdB2C)} -> {nameof(AzureAdB2CSettings.AzureAdB2C.B2cExtensionAppClientId)}.",
                nameof(config));
        }

        _graphClient = graphClient;
        _logger = logger;
        var extensionAppClientId = config.Value.AzureAdB2C.B2cExtensionAppClientId;

        RoleAttributeName = $"extension_{extensionAppClientId.Replace("-", "")}_{RoleAttribute}";
    }

    public async Task GetRolesViaGraphApi(IEnumerable<RemoteAccount> remoteAccounts)
    {
        foreach (var remoteAccount in remoteAccounts)
        {
            await GetRoleViaGraphApi(remoteAccount).ConfigureAwait(false);
        }
    }

    private async Task GetRoleViaGraphApi(RemoteAccount remoteAccount)
    {
        var role = Role.Guest;

        try
        {
            var user = await _graphClient.Users[remoteAccount.ObjectId.ToString()]
                .Request().Select($"id,displayName,{RoleAttributeName}")
                .GetAsync().ConfigureAwait(false);

            role = TryToGetRoleFromCustomAttribute(user);
        }
        catch (ServiceException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("User with ID {RemoteAccount} was not found in AADB2C, assigning guest role",
                    remoteAccount.ObjectId);
            }
            else
            {
                _logger.LogError(e, "Failed to query user with ID {RemoteAccount} from AADB2C, assigning guest role",
                    remoteAccount.ObjectId);
            }
        }

        remoteAccount.Role = role;
    }

    private Role TryToGetRoleFromCustomAttribute(User user)
    {
        if (user.AdditionalData == null || !user.AdditionalData.TryGetValue(RoleAttributeName, out var roleValue))
        {
            _logger.LogInformation("Role attribute is missing for user {User}", user.DisplayName);
            return Role.Guest;
        }
        
        var roleString = roleValue?.ToString();
        var parseSuccess = Enum.TryParse<Role>(roleString, out var role);
        if (parseSuccess && role.IsDefinedEnumValue())
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
            await _graphClient.Users[userId].Request().UpdateAsync(user).ConfigureAwait(false);
            _logger.LogInformation("Updated role to '{Role}' for user with object ID '{UserId}'",
                role, userId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, 
                "Failed to update role to '{Role}' for user with object ID '{UserId}'",
                role, userId);
        }
    }
}