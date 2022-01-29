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

    public UserService(IGraphServiceClient graphClient, IOptions<AzureAdB2CSettings> config, ILogger<UserService> logger)
    {
        if (string.IsNullOrWhiteSpace(config?.Value?.AzureAdB2C?.B2cExtensionAppClientId))
        {
            throw new ArgumentException(
                "B2C Extension App ClientId (ApplicationId) is missing in the appsettings.json.",
                nameof(_extensionAppClientId));
        }

        _graphClient = graphClient;
        _logger = logger;
        _extensionAppClientId = config.Value.AzureAdB2C.B2cExtensionAppClientId;
    }

    public async Task UpdateRoleViaGraphApi(IEnumerable<RemoteAccount> remoteAccounts)
    {
        var roleAttributeName = GetCompleteAttributeName("Role");

        foreach (var remoteAccount in remoteAccounts)
        {
            var role = Role.Guest;

            try
            {
                var user = await _graphClient.Users[remoteAccount.ObjectId.ToString()]
                    .Request()
                    .Select($"id,displayName,{roleAttributeName}")
                    .GetAsync();
            
                if (user.AdditionalData?[roleAttributeName] != null)
                {
                    var roleString = user.AdditionalData?[roleAttributeName].ToString();
                    role = Enum.Parse<Role>(roleString);
                }
            }
            catch (ServiceException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User with ID {0} was not found in AADB2C, assigning guest role",
                        remoteAccount.ObjectId);
                }
                else
                {
                    _logger.LogError(e, "Failed to query user with ID {0} from AADB2C, assigning guest role",
                        remoteAccount.ObjectId);
                }
            }

            remoteAccount.Role = role;
        }
    }
   
    public async Task UpdateUserRole(string userId, Role role)
    {

        Console.WriteLine($"Looking for user with object ID '{userId}'...");

        // Declare the names of the custom attributes
        const string customAttributeName1 = "Role";

        // Get the complete name of the custom attribute (Azure AD extension)
        string roleAttributeName = GetCompleteAttributeName(customAttributeName1);

        Console.WriteLine($"Create a user with the custom attributes '{customAttributeName1}'");

        // Fill custom attributes
        IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
        extensionInstance.Add(roleAttributeName, ((int)role).ToString());

        var user = new Microsoft.Graph.User
        {
            AdditionalData = extensionInstance
        };


        try
        {
            // Update user by object ID
            await _graphClient.Users[userId]
                .Request()
                .UpdateAsync(user);

            Console.WriteLine($"User with object ID '{userId}' successfully updated.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }

    internal string GetCompleteAttributeName(string attributeName)
    {
        var sanitizedClientId = _extensionAppClientId.Replace("-", "");
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new System.ArgumentException("Parameter cannot be null", nameof(attributeName));
        }

        return $"extension_{sanitizedClientId}_{attributeName}";
    }
}