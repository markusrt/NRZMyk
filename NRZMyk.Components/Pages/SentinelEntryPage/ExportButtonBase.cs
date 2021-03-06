﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class ExportButtonBase : ComponentBase
    {
        [Inject]
        internal IJSRuntime JsRuntime { get; set; }   

        [Inject]
        internal  SentinelEntryService SentinelEntryService { get; set; }

        internal bool DownloadInProgress { get; private set; }
        
        protected async Task DownloadFile()
        {
            DownloadInProgress = true;
            var fileData = await SentinelEntryService.Export();
            var fileName =  $"Sentinel-Export_{DateTime.Now:yyyyMMdd}.xlsx";
            await JsRuntime.InvokeAsync<object>("saveAsFile", new object[] { fileName, fileData });
            DownloadInProgress = false;
        }
    }
}