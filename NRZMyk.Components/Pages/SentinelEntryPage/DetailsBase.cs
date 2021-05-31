using System;
using System.Threading.Tasks;
using AutoMapper;
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
        private ILogger<CreateBase> Logger { get; set; }

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        internal SentinelEntry SentinelEntry { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /SentinelEntries/Details/{Id}", Id);

            SentinelEntry = await SentinelEntryService.GetById(Id);

            await base.OnInitializedAsync();
        }
    }
}