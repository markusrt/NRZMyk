using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
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
        internal IJSRuntime JsRuntime { get; set; } = default!;  
        
        [Inject]
        private IMapper Mapper { get; set; } = default!;

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        [Inject]
        private ILogger<CryoViewBase> Logger { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; set; } = default!;

        internal List<SentinelEntryResponse> SentinelEntries { get; set; } = default!;

        internal int SelectedOrganization { get; set; }

        internal bool ShowEdit { get; private set; }

        internal int SelectedId { get; private set; }
        
        protected LoadState LoadState { get; set; }

        private readonly List<int> _updatingItems = new();
        private readonly Dictionary<int, string> _originalCryoRemarks = new();
        
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

        internal async Task UpdateCryoRemark(SentinelEntryResponse entry)
        {
            _updatingItems.Add(entry.Id);
            await SentinelEntryService.UpdateCryoRemark(new CryoRemarkUpdateRequest
            {
                Id = entry.Id,
                CryoRemark = entry.CryoRemark
            }).ConfigureAwait(true);

            // Update the original value after successful save
            _originalCryoRemarks[entry.Id] = entry.CryoRemark ?? string.Empty;
            
            await RefreshEntryAndUpdateState(entry).ConfigureAwait(true);
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

            await RefreshEntryAndUpdateState(entry).ConfigureAwait(true);
        }

        private async Task RefreshEntryAndUpdateState(SentinelEntryResponse entry)
        {
            var index = SentinelEntries.IndexOf(entry);
            var updatedEntry = await SentinelEntryService.GetById(entry.Id).ConfigureAwait(true);
            SentinelEntries[index] = updatedEntry;
            _updatingItems.Remove(entry.Id);
            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }

        internal void EditClick(int id)
        {
            SelectedId = id;
            ShowEdit = true;
        }
        
        internal async Task CloseEditHandler(string action)
        {
            var updatedEntry = await SentinelEntryService.GetById(SelectedId).ConfigureAwait(true);
            var index = SentinelEntries.IndexOf(SentinelEntries.Single(s => s.Id == SelectedId));
            SentinelEntries[index] =  updatedEntry;
            await JsRuntime.InvokeAsync<object>("closeBootstrapModal", new object[] { "editModal" }).ConfigureAwait(true);
            ShowEdit = false;
            SelectedId = 0;
            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }

        internal async Task LoadData()
        {
            LoadState = LoadState.Loading;

            SentinelEntries = Mapper.Map<List<SentinelEntryResponse>>(
                await SentinelEntryService.ListByOrganization(SelectedOrganization).ConfigureAwait(true));

            // Initialize original cryo remarks for tracking changes
            _originalCryoRemarks.Clear();
            foreach (var entry in SentinelEntries)
            {
                _originalCryoRemarks[entry.Id] = entry.CryoRemark ?? string.Empty;
            }

            LoadState = LoadState.Loaded;

            await InvokeAsync(StateHasChanged).ConfigureAwait(true);
        }

        internal bool IsUpdating(SentinelEntryResponse entry)
        {
            return _updatingItems.Contains(entry.Id);
        }
        
        internal bool HasCryoRemarkChanged(SentinelEntryResponse entry)
        {
            if (!_originalCryoRemarks.ContainsKey(entry.Id))
            {
                _originalCryoRemarks[entry.Id] = entry.CryoRemark ?? string.Empty;
                return false;
            }
            
            return (_originalCryoRemarks[entry.Id] ?? string.Empty) != (entry.CryoRemark ?? string.Empty);
        }
    }
}