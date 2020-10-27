using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class CreateBase : BlazorComponent
    {
        [Parameter]
        public int? Id { get; set; }

        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        [Parameter]
        public EventCallback<ChangeEventArgs> ValueChanged { get; set; }

        [Inject]
        private ILogger<CreateBase> Logger { get; set; }

        [Inject]
        private SentinelEntryService SentinelEntryService { get; set; }

        [Inject]
        protected MicStepsService MicStepsService { get; set; }

        [Inject]
        private ClinicalBreakpointService ClinicalBreakpointService { get; set; }

        public SentinelEntryRequest SentinelEntry { get; private set; }

        public List<ClinicalBreakpoint> AllBreakpoints { get; private set; } = new List<ClinicalBreakpoint>();
        
        public SpeciesTestingMethod TestingMethod { get; set; } = SpeciesTestingMethod.Vitek;
        
        public BrothMicrodilutionStandard Standard { get; set; } = BrothMicrodilutionStandard.Eucast;

        public AntifungalAgent AntifungalAgent { get; set; } = AntifungalAgent.Micafungin;

        public string Title { get; set; }
        
        public string PrimaryAction { get; set; }
        
        public bool SaveFailed { get; set; }

        internal void AddAntimicrobialSensitivityTest()
        {
            var antifungalAgents = MicStepsService.IsMultiAgentSystem(TestingMethod)
                ? MicStepsService.AntifungalAgents(TestingMethod)
                : new List<AntifungalAgent> {AntifungalAgent};
            
            foreach (var antifungalAgent in antifungalAgents)
            {
                AddAntimicrobialSensitivityTest(new AntimicrobialSensitivityTestRequest
                {
                    TestingMethod = TestingMethod,
                    AntifungalAgent = antifungalAgent,
                    Standard = Standard
                });
            }
        }

        private void AddAntimicrobialSensitivityTest(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            SentinelEntry.AntimicrobialSensitivityTests.Add(sensitivityTest);
            sensitivityTest.ClinicalBreakpointId = ApplicableBreakpoints(sensitivityTest).FirstOrDefault()?.Id;
        }

        internal void RemoveAntimicrobialSensitivityTest(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            SentinelEntry.AntimicrobialSensitivityTests.Remove(sensitivityTest);
        }

        internal IEnumerable<MicStep> MicSteps(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            var matchingSteps = MicStepsService.StepsByTestingMethodAndAgent(sensitivityTest.TestingMethod, sensitivityTest.AntifungalAgent);
            if (sensitivityTest.MinimumInhibitoryConcentration.HasValue && matchingSteps.Any() && matchingSteps.All(s => !s.Value.Equals(sensitivityTest.MinimumInhibitoryConcentration)))
            {
                sensitivityTest.MinimumInhibitoryConcentration = matchingSteps.First().Value;
            }
            return matchingSteps;
        }

        internal IEnumerable<SpeciesTestingMethod> TestingMethods()
        {
            return MicStepsService.TestingMethods();
        }

        internal IEnumerable<AntifungalAgent> AntifungalAgents()
        {
            return MicStepsService.AntifungalAgents(TestingMethod);
        }

        internal IEnumerable<BrothMicrodilutionStandard> Standards()
        {
            var standards = MicStepsService.Standards(TestingMethod).ToList();
            if (!standards.Contains(Standard))
            {
                Standard = standards.First();
            }
            return standards;
        }

        internal IEnumerable<ClinicalBreakpoint> ApplicableBreakpoints(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            var antifungalAgent = sensitivityTest.AntifungalAgent;
            var applicableBreakpoints = AllBreakpoints.Where(b => 
                b.AntifungalAgent == antifungalAgent 
                && b.Species == SentinelEntry.IdentifiedSpecies
                && b.Standard == sensitivityTest.Standard).ToList();
            
            if (applicableBreakpoints.All(b => b.Id != sensitivityTest.ClinicalBreakpointId))
            {
                Logger.LogInformation($"Update test {sensitivityTest.AntifungalAgent} to breakpoint id {applicableBreakpoints.FirstOrDefault()?.Id}");
                sensitivityTest.ClinicalBreakpointId = applicableBreakpoints.FirstOrDefault()?.Id;
            }

            Logger.LogInformation($"Found {applicableBreakpoints.Count} applicable breakpoints for {antifungalAgent} and {SentinelEntry.IdentifiedSpecies}");
            return applicableBreakpoints;
        }

        internal string DuplicateClass(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            var isDuplicate = SentinelEntry.AntimicrobialSensitivityTests.Count(
                s => s.AntifungalAgent == sensitivityTest.AntifungalAgent
                     && s.Standard == sensitivityTest.Standard
                     && s.TestingMethod == sensitivityTest.TestingMethod) > 1;
            return isDuplicate ? "duplicate-sensitivity-test" : "";
        }

        internal string ResistanceBadge(AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            Logger.LogInformation("Resistance update");
            var breakpoint = AllBreakpoints.FirstOrDefault(b => b.Id == sensitivityTest.ClinicalBreakpointId);
            if (breakpoint?.MicBreakpointResistent == null || !breakpoint.MicBreakpointSusceptible.HasValue)
            {
                if (breakpoint == null)
                {
                    Logger.LogWarning($"No breakpoint found for {sensitivityTest.TestingMethod}/{sensitivityTest.AntifungalAgent} where id is {sensitivityTest.ClinicalBreakpointId}");
                }
                else
                {
                    Logger.LogInformation($"Breakpoints {breakpoint.Id} (resistent/suseptible) values are not complete ({breakpoint.MicBreakpointResistent}/{breakpoint.MicBreakpointSusceptible})");
                }
                sensitivityTest.Resistance = Resistance.NotDetermined;
                return "badge-info";
            }

            Logger.LogInformation($"Found breakpoint for {sensitivityTest.TestingMethod}/{sensitivityTest.AntifungalAgent} where id is {sensitivityTest.ClinicalBreakpointId}: {breakpoint.Title}");

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

        internal async Task SubmitClick()
        {
            try
            {
                if (IsEdit())
                {
                    await SentinelEntryService.Update(SentinelEntry);
                }
                else
                {
                    await SentinelEntryService.Create(SentinelEntry);
                }

                SaveFailed = false;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Storing failed");
                SaveFailed = true;
            }

            if (!SaveFailed)
            {
                await OnCloseClick.InvokeAsync(null);
            }
        }

        protected IEnumerable<AntimicrobialSensitivityTestRequest> RecalculateResistance()
        {
            return SentinelEntry.AntimicrobialSensitivityTests;
        }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation($"Now loading... /SentinelEntry/{(IsEdit()?"Edit":"Create")}");

            Title = IsEdit() ? "Bearbeiten" : "Neu anlegen";
            PrimaryAction = IsEdit() ? "Speichern" : "Anlegen";

            AllBreakpoints = await ClinicalBreakpointService.List();

            if (Id.HasValue)
            {
                SentinelEntry = await SentinelEntryService.GetById(Id.Value);
            }
            else
            {
                SentinelEntry = new SentinelEntryRequest();
            }

            await base.OnInitializedAsync();
        }

        private bool IsEdit()
        {
            return Id.HasValue;
        }
    }
}
