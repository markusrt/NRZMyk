﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private IAccountService AccountService { get; set; }

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }

        [Inject]
        private ILogger<CryoViewBase> Logger { get; set; }

        internal ICollection<Organization> Organizations { get; set; }

        protected List<SentinelEntry> SentinelEntries { get; set; }

        protected int SelectedOrganization { get; set; }
        
        protected bool DataLoaded { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Logger.LogInformation("Now loading... /cryo-view/sentinel-entries");

                SentinelEntries = await SentinelEntryService.ListByOrganization(SelectedOrganization);
                Organizations = await AccountService.ListOrganizations();
                SelectedOrganization = Organizations.First().Id;

                await InvokeAsync(CallRequestRefresh);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        internal async Task PutToCryoStorage(SentinelEntry entry)
        {
            await SentinelEntryService.CryoArchive(new CryoArchiveRequest
            {
                Id = entry.Id,
                CryoDate = DateTime.Now,
                CryoRemark = entry.CryoRemark
            });
            await LoadData();
        }


        internal async Task ReleaseFromCryoStorage(SentinelEntry entry)
        {
            await SentinelEntryService.CryoArchive(new CryoArchiveRequest
            {
                Id = entry.Id,
                CryoDate = null,
                CryoRemark = entry.CryoRemark
            });
            await LoadData();
        }


        internal async Task LoadData()
        {
            SentinelEntries = await SentinelEntryService.ListByOrganization(SelectedOrganization);
            DataLoaded = true;
            StateHasChanged();
        }
    }
}