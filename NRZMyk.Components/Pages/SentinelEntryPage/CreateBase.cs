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
        private MicStepsService MicStepsService { get; set; }

        [Inject]
        private ClinicalBreakpointService ClinicalBreakpointService { get; set; }

        public CreateSentinelEntryRequest NewSentinelEntry { get; } = new CreateSentinelEntryRequest();

        public List<ClinicalBreakpoint> AllBreakpoints { get; private set; } = new List<ClinicalBreakpoint>();

        public SpeciesTestingMethod TestingMethod { get; set; } = SpeciesTestingMethod.Vitek;

        public AntifungalAgent AntifungalAgent { get; set; } = AntifungalAgent.Micafungin;

        public void AddAntimicrobialSensitivityTest()
        {
            NewSentinelEntry.SensitivityTests.Add(
                new AntimicrobialSensitivityTest
                {
                    TestingMethod = TestingMethod,
                    AntifungalAgent = AntifungalAgent,
                    ClinicalBreakpointId = AllBreakpoints.First().Id
                });
        }

        protected IEnumerable<AntimicrobialSensitivityTest> RecalculateResistance()
        {
            return NewSentinelEntry.SensitivityTests;
        }

        public List<MicStep> MicSteps(SpeciesTestingMethod testingMethod, AntifungalAgent antifungalAgent)
        {
            return MicStepsService.StepsByTestingMethodAndAgent(testingMethod, antifungalAgent);
        }

        public IEnumerable<ClinicalBreakpoint> ApplicableBreakpoints(AntifungalAgent antifungalAgent)
        {
            return AllBreakpoints.Where(b => b.AntifungalAgent == antifungalAgent && b.Species == NewSentinelEntry.IdentifiedSpecies);
        }

        public string ResistenceBadge(AntimicrobialSensitivityTest sensitivityTest)
        {
            var breakpoint = AllBreakpoints.FirstOrDefault(b => b.Id == sensitivityTest.ClinicalBreakpointId);
            if (breakpoint == null || !breakpoint.MicBreakpointResistent.HasValue || !breakpoint.MicBreakpointSusceptible.HasValue)
            {
                sensitivityTest.Resistance = Resistance.NotDetermined;
                return "badge-info";
            }

            var mic = sensitivityTest.MinimumInhibitoryConcentration;
            if (mic > breakpoint.MicBreakpointResistent)
            {
                sensitivityTest.Resistance = Resistance.Resistant;
                return "badge-danger";
            }
            if (mic <= breakpoint.MicBreakpointSusceptible)
            {
                sensitivityTest.Resistance = Resistance.Susceptible;
                return "badge-success";
            }
            sensitivityTest.Resistance = Resistance.Intermediate;
            return "badge-warning";
        }

        public async Task CreateClick()
        {
            await SentinelEntryService.Create(NewSentinelEntry);
            await OnCloseClick.InvokeAsync(null);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Logger.LogInformation("Now loading... /SentinelEntry/Create");
            if (firstRender)
            {
                AllBreakpoints = await ClinicalBreakpointService.List();

                CallRequestRefresh();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
