using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages
{
    public class AdminBase : BlazorComponent
    {
        [Inject]
        private IAccountService AccountService { get; set; } = default!;

        [Inject]
        private ILogger<AdminBase> Logger { get; set; } = default!;

        internal ICollection<RemoteAccount> Accounts { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; set; } = default!;
        
        internal SaveState SaveState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /Admin");

            Accounts = await AccountService.ListAccounts().ConfigureAwait(true);
            Organizations = await AccountService.ListOrganizations().ConfigureAwait(true);
            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        internal async Task SubmitClick()
        {
            try
            {
                await AccountService.AssignToOrganization(Accounts).ConfigureAwait(true);
                SaveState = SaveState.Success;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Update of accounts failed");
                SaveState = SaveState.Failed;
            }
        }
    }
}