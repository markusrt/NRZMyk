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
        private IAccountService AccountService { get; set; } = default!;

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; } = default!;

        [Inject]
        private ILogger<CryoViewBase> Logger { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; set; } = default!;

        protected List<SentinelEntry> SentinelEntries { get; set; } = default!;

        internal int SelectedOrganization { get; set; }
        
        protected LoadState LoadState { get; set; }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation("Now loading... /cryo-view/sentinel-entries");

                Organizations = await AccountService.ListOrganizations().ConfigureAwait(true);
                SelectedOrganization = Organizations.First().Id;
                SentinelEntries = new List<SentinelEntry>();

                await InvokeAsync(CallRequestRefresh).ConfigureAwait(true);
            }

            await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        }

        internal async Task PutToCryoStorage(SentinelEntry entry)
        {
            LoadState = LoadState.Loading;
            await SentinelEntryService.CryoArchive(new CryoArchiveRequest
            {
                Id = entry.Id,
                CryoDate = DateTime.Now,
                CryoRemark = entry.CryoRemark
            }).ConfigureAwait(true);
            await LoadData().ConfigureAwait(true);
        }


        internal async Task ReleaseFromCryoStorage(SentinelEntry entry)
        {
            LoadState = LoadState.Loading;
            await SentinelEntryService.CryoArchive(new CryoArchiveRequest
            {
                Id = entry.Id,
                CryoDate = null,
                CryoRemark = entry.CryoRemark
            }).ConfigureAwait(true);
            await LoadData().ConfigureAwait(true);
        }


        internal async Task LoadData()
        {
            LoadState = LoadState.Loading;

            SentinelEntries = await SentinelEntryService.ListByOrganization(SelectedOrganization).ConfigureAwait(true);

            LoadState = LoadState.Loaded;

            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }
    }
}