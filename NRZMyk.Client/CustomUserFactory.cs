using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace NRZMyk.Client
{
    public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly ILogger<CustomUserFactory> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public CustomUserFactory(IAccessTokenProviderAccessor accessor, IHttpClientFactory clientFactory, ILogger<CustomUserFactory> logger)
            : base(accessor)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options).ConfigureAwait(true);
            var token = await TokenProvider.RequestAccessToken().ConfigureAwait(true);
            var tokenStatus = token.Status;

            if (initialUser.Identity is not ClaimsIdentity userIdentity 
                || tokenStatus != AccessTokenResultStatus.Success
                || !userIdentity.IsAuthenticated)
            {
                return initialUser;
            }

            _logger.LogInformation("Connecting logged in user");
            
            userIdentity.AddRolesFromExtensionClaim();
            
            try
            {
                var client = _clientFactory.CreateClient("NRZMyk.ServerAPI");
                var response = await client.GetAsync("api/user/connect").ConfigureAwait(true);
                if (response.IsSuccessStatusCode)
                {
                    var connectedAccount =
                        await response.Content.ReadFromJsonAsync<ConnectedAccount>().ConfigureAwait(true);

                    if (connectedAccount != null)
                    {
                        _logger.LogInformation("Connect success, {displayName}, IsGuest={isGuest}", connectedAccount.Account.DisplayName, connectedAccount.IsGuest);
                        if (!connectedAccount.IsGuest && !userIdentity.HasClaim(ClaimTypes.Role, nameof(Role.User)))
                        {
                            userIdentity.AddClaim(new Claim(ClaimTypes.Role, nameof(Role.User)));
                            userIdentity.RemoveClaim(userIdentity.Claims.FirstOrDefault(c =>
                                c.Type == ClaimTypes.Role && c.Value == nameof(Role.Guest)));
                        }
                    }
                    else
                    {
                        _logger.LogError("Failed to connect account as it deserialized as 'null'");
                    }
                }
                else
                {
                    _logger.LogError("Connect API request failed with status code: {statusCode}", response.StatusCode);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Connect API request failed: {errorMessage}", exception.Message);
            }

            return initialUser;
        }
    }
}