using AutoMapper;
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
        private IMapper Mapper { get; set; } = default!;

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        [Inject]
        private ILogger<CryoViewBase> Logger { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; set; } = default!;

        internal List<SentinelEntryResponse> SentinelEntries { get; set; } = default!;

        internal int SelectedOrganization { get; set; }
        
        protected LoadState LoadState { get; set; }

        private readonly List<int> _updatingItems = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation("Now loading... /cryo-view/sentinel-entries");

                Organizations = await AccountService.ListOrganizations().ConfigureAwait(true);
                SelectedOrganization = Organizations.First().Id;
                SentinelEntries = new List<SentinelEntryResponse>();

                await InvokeAsync(CallRequestRefresh).ConfigureAwait(true);
            }

            await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);
        }

        internal Task PutToCryoStorage(SentinelEntryResponse entry)
        {
            return CryoStore(entry, true);
        }

        internal Task ReleaseFromCryoStorage(SentinelEntryResponse entry)
        {
            return CryoStore(entry, false);
        }

        private async Task CryoStore(SentinelEntryResponse entry, bool store)
        {
            _updatingItems.Add(entry.Id);
            await SentinelEntryService.CryoArchive(new CryoArchiveRequest
            {
                Id = entry.Id,
                CryoDate = store ? DateTime.Now : null,
                CryoRemark = entry.CryoRemark
            }).ConfigureAwait(true);

            var index = SentinelEntries.IndexOf(entry);
            var updatedEntry = await SentinelEntryService.GetById(entry.Id);
            SentinelEntries[index] =  updatedEntry;
            _updatingItems.Remove(entry.Id);
            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }

        internal async Task LoadData()
        {
            LoadState = LoadState.Loading;

            SentinelEntries = Mapper.Map<List<SentinelEntryResponse>>(
                await SentinelEntryService.ListByOrganization(SelectedOrganization).ConfigureAwait(true));

            LoadState = LoadState.Loaded;

            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }

        internal bool IsUpdating(SentinelEntryResponse entry)
        {
            return _updatingItems.Contains(entry.Id);
        }
    }
}