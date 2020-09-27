using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class ExportFilterBase : ComponentBase
    {
        [Inject]
        internal IJSRuntime JsRuntime { get; set; }   

        [Inject]
        internal  SentinelEntryService SentinelEntryService { get; set; }

        internal int IsDownloadStarted { get; private set; }
        
        protected async Task DownloadFile()
        {
            IsDownloadStarted = 1;
            var fileData = await SentinelEntryService.Export();
            var fileName =  $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx";
            await JsRuntime.InvokeAsync<object>("saveAsFile", new object[] { fileName, fileData });
            IsDownloadStarted = 2;
        }
    }
}