using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class ExportButtonBase : ComponentBase
    {
        [Inject]
        internal IJSRuntime JsRuntime { get; set; } = default!;   

        [Inject]
        internal  ISentinelEntryService SentinelEntryService { get; set; } = default!;

        internal bool DownloadInProgress { get; private set; }
        
        protected async Task DownloadFile()
        {
            DownloadInProgress = true;
            var fileData = await SentinelEntryService.Export().ConfigureAwait(true);
            var fileName =  $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx";
            await JsRuntime.InvokeAsync<object>("saveAsFile", new object[] { fileName, fileData }).ConfigureAwait(true);
            DownloadInProgress = false;
        }
    }
}