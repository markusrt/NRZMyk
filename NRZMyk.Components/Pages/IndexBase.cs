using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Services;
using Organization = NRZMyk.Services.Data.Entities.Organization;

namespace NRZMyk.Components.Pages
{
    public class IndexBase : BlazorComponent
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        private IAccountService AccountService { get; set; } = default!;

        [Inject]
        internal IReminderService ReminderService { get; set; } = default!;

        [Inject]
        private ILogger<IndexBase> Logger { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; private set; } = new List<Organization>();
        
        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /Index");
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authenticationState.User.Identity.IsAuthenticated)
            {
                Organizations = await AccountService.ListOrganizations().ConfigureAwait(true);
            }
            await base.OnInitializedAsync().ConfigureAwait(true);
        }
    }
}