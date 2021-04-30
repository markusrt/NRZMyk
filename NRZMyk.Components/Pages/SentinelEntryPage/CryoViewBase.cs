using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class CryoViewBase : BlazorComponent
    {
        [Inject]
        private IAccountService AccountService { get; set; }

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }

        [Inject]
        private ILogger<CryoViewBase> Logger { get; set; }

        internal ICollection<Organization> Organizations { get; set; }

        protected List<SentinelEntry> SentinelEntries { get; set; }

        protected int SelectedOrganization { get; set; } = -1;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation("Now loading... /cryo-view/sentinel-entries");

                SentinelEntries = await SentinelEntryService.ListByOrganization(SelectedOrganization);
                Organizations = await AccountService.ListOrganizations();

                await InvokeAsync(CallRequestRefresh);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        internal async Task StoreClick(SentinelEntry entry)
        {
            //Store
            await ReloadCatalogItems();
        }

        internal async Task ReloadCatalogItems()
        {
            SentinelEntries = await SentinelEntryService.ListByOrganization(SelectedOrganization);
            StateHasChanged();
        }
    }
}