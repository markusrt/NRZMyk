using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class DeleteBase : BlazorComponent
    {
        [Inject]
        private ILogger<CreateBase> Logger { get; set; } = default!;

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        [Inject]
        private IMapper Mapper { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        internal SentinelEntryRequest SentinelEntry { get; set; } = default!;

        internal bool DeleteFailed { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /Catalog/Delete/{Id}", Id);

            SentinelEntry = Mapper.Map<SentinelEntryRequest>(await SentinelEntryService.GetById(Id).ConfigureAwait(true));

            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        internal async Task DeleteClick()
        {
            try
            {
                await SentinelEntryService.Delete(Id).ConfigureAwait(true);
                DeleteFailed = false;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Deleting failed");
                DeleteFailed = true;
            }

            if (!DeleteFailed)
            {
                await OnCloseClick.InvokeAsync(null).ConfigureAwait(true);
            }
        }
    }
}