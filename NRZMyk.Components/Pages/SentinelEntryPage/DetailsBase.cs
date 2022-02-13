using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class DetailsBase : BlazorComponent
    {
        [Inject]
        private ILogger<CreateBase> Logger { get; set; } = default!;

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        internal SentinelEntry SentinelEntry { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /SentinelEntries/Details/{Id}", Id);

            SentinelEntry = await SentinelEntryService.GetById(Id).ConfigureAwait(true);

            await base.OnInitializedAsync().ConfigureAwait(true);
        }
    }
}