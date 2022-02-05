using Microsoft.Graph.ExternalConnectors;

namespace NRZMyk.Services.Configuration;

public class AzureAdB2C
{
    public string Instance { get; set; }
    public string ClientId { get; set; }
    public string B2cExtensionAppClientId { get; set; }
    public string Domain { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string ClientSecret { get; set; }
}