using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Services;

namespace NRZMyk.Server.Utils;

public class GraphServiceClientWrapper : IGraphServiceClient
{
    private static readonly string[] Scopes = { "https://graph.microsoft.com/.default" };
    
    private readonly GraphServiceClient _graphClient;

    public IGraphServiceUsersCollectionRequestBuilder Users => _graphClient.Users;

    public GraphServiceClientWrapper(IOptions<AzureAdB2CSettings> config)
    {
        var settings = config.Value.AzureAdB2C;
        var clientSecretCredential = new ClientSecretCredential(settings.Domain, settings.ClientId, settings.ClientSecret);
        _graphClient = new GraphServiceClient(clientSecretCredential, Scopes);
    }
}