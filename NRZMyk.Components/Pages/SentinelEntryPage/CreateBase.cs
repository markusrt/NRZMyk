using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Models;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class CreateBase : BlazorComponent
    {
        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs> ValueChanged { get; set; }

        [Inject] 
        private ILogger<CreateBase> Logger { get; set; }
        
        [Inject] 
        private SentinelEntryService SentinelEntryService { get; set; }

        [Inject] 
        private ClinicalBreakpointService ClinicalBreakpointService { get; set; }

        protected readonly CreateSentinelEntryRequest _item = new CreateSentinelEntryRequest();
        
        protected List<ClinicalBreakpoint> _clinicalBreakpoints = new List<ClinicalBreakpoint>();

        protected SpeciesTestingMethod _testingMethod;
        
        protected AntifungalAgent _antifungalAgent;

        public void AddAntimicrobialSensitivityTest()
        {
            _item.SensitivityTests.Add(
                new AntimicrobialSensitivityTest
                {
                    TestingMethod = _testingMethod, 
                    AntifungalAgent = _antifungalAgent, 
                    EucastClinicalBreakpointId = _clinicalBreakpoints.First().Id
                });
        }

        protected IEnumerable<AntimicrobialSensitivityTest> RecalculateResistance()
        {
            return _item.SensitivityTests;
        }

        public string ResistenceBadge(AntimicrobialSensitivityTest sensitivityTest)
        {
            if (sensitivityTest.Resistance == Resistance.Susceptible)
            {
                sensitivityTest.Resistance = Resistance.Resistant;
            }
            else
            {
                sensitivityTest.Resistance = Resistance.Susceptible;
            }


            var resistance = sensitivityTest.Resistance;
            if (resistance == Resistance.Resistant)
            {
                return "badge-danger";
            }
            if (resistance == Resistance.Intermediate)
            {
                return "badge-warning";
            }
            if (resistance == Resistance.Susceptible)
            {
                return "badge-success";
            }
            return "badge-info";
        }

        public async Task CreateClick()
        {
            await SentinelEntryService.Create(_item);
            await OnCloseClick.InvokeAsync(null);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Logger.LogInformation("Now loading... /SentinelEntry/Create");
            if (firstRender)
            {
                _clinicalBreakpoints = await ClinicalBreakpointService.List();

                CallRequestRefresh();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
