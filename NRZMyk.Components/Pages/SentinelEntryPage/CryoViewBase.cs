using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class CryoViewBase : BlazorComponent
    {
        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }
        
        protected List<SentinelEntry> sentinelEntries;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                sentinelEntries = await SentinelEntryService.ListPaged(50);

                await InvokeAsync(CallRequestRefresh);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        internal async Task StoreClick(int id)
        {
            //Store
            await ReloadCatalogItems();
        }

        private async Task ReloadCatalogItems()
        {
            sentinelEntries = await SentinelEntryService.ListPaged(50);
            StateHasChanged();
        }
    }
}