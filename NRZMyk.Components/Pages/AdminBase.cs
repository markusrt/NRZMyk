using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Components.SharedComponents;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages
{
    public class AdminBase : BlazorComponent
    {
        [Inject]
        private IAccountService AccountService { get; set; }

        [Inject]
        private ILogger<AdminBase> Logger { get; set; }

        internal ICollection<RemoteAccount> Accounts { get; set; }

        internal ICollection<Organization> Organizations { get; set; }
        
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