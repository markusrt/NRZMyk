using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Components.Pages.SentinelEntryPage
{
    public class CreateBase : BlazorComponent
    {
        private const float EucastExtraLowSusceptibleValueToAlwaysGetIntermediate = 0.001f;

        [Parameter]
        public int? Id { get; set; }

        [Parameter]
        public EventCallback<string> OnCloseClick { get; set; }

        private bool OnCloseSet => OnCloseClick.HasDelegate;

        [Parameter]
        public EventCallback<ChangeEventArgs> ValueChanged { get; set; }

        [Inject]
        private ILogger<CreateBase> Logger { get; set; } = default!;

        [Inject]
        private ISentinelEntryService SentinelEntryService { get; set; } = default!;

        [Inject]
        protected IMicStepsService MicStepsService { get; set; } = default!;

        [Inject]
        private IClinicalBreakpointService ClinicalBreakpointService { get; set; } = default!;
        
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        public SentinelEntryRequest SentinelEntry { get; private set; } = default!;

        public List<ClinicalBreakpoint> AllBreakpoints { get; private set; } = new List<ClinicalBreakpoint>();
        
        public SpeciesTestingMethod TestingMethod { get; set; } = SpeciesTestingMethod.Vitek;
        
        public BrothMicrodilutionStandard Standard { get; set; } = BrothMicrodilutionStandard.Eucast;

        public AntifungalAgent AntifungalAgent { get; set; } = AntifungalAgent.Micafungin;

        public string Title { get; set; } = default!;
        
        public string PrimaryAction { get; set; } = default!;

        public bool SaveFailed => !string.IsNullOrEmpty(LastError);

        public string LastError { get; set; } = string.Empty;

        internal string CryoBox { get; set; } = string.Empty;

        internal string LaboratoryNumber { get; set; } = string.Empty;

        internal void AddSub()
        {
            SentinelEntry.Subs.Add(new Sub());
        }

        internal void AddAntimicrobialSensitivityTest(Sub sub)
        {
            var antifungalAgents = MicStepsService.IsMultiAgentSystem(TestingMethod)
                ? MicStepsService.AntifungalAgents(TestingMethod)
                : new List<AntifungalAgent> {AntifungalAgent};
            
            foreach (var antifungalAgent in antifungalAgents)
            {
                AddAntimicrobialSensitivityTest(sub, new AntimicrobialSensitivityTestRequest
                {
                    TestingMethod = TestingMethod,
                    AntifungalAgent = antifungalAgent,
                    Standard = Standard
                });
            }
        }

        private void AddAntimicrobialSensitivityTest(Sub sub, AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            sub.AntimicrobialSensitivityTests.Add(sensitivityTest);
            sensitivityTest.ClinicalBreakpointId = ApplicableBreakpoints(sensitivityTest).FirstOrDefault()?.Id;
        }

        internal void RemoveAntimicrobialSensitivityTest(Sub sub, AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            sub.AntimicrobialSensitivityTests.Remove(sensitivityTest);
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
            
            if (!applicableBreakpoints.Any())
            {
                sensitivityTest.Resistance = Resistance.NotDetermined;
            }
            return applicableBreakpoints;
        }

        internal string DuplicateClass(Sub sub, AntimicrobialSensitivityTestRequest sensitivityTest)
        {
            var isDuplicate = sub.AntimicrobialSensitivityTests.Count(
                s => s.AntifungalAgent == sensitivityTest.AntifungalAgent
                     && s.Standard == sensitivityTest.Standard
                     && s.TestingMethod == sensitivityTest.TestingMethod) > 1;
            return isDuplicate ? "duplicate-sensitivity-test" : "";
        }

        //TODO move resistance evaluation to a pure business logic class
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
                    Logger.LogInformation($"Breakpoints {breakpoint.Id} (resistant/susceptible) values are not complete ({breakpoint.MicBreakpointResistent}/{breakpoint.MicBreakpointSusceptible})");
                }
                sensitivityTest.Resistance = Resistance.NotDetermined;
                return "bg-info";
            }

            var selectedStep = MicStepsService.StepsByTestingMethodAndAgent(sensitivityTest.TestingMethod, sensitivityTest.AntifungalAgent)
                .FirstOrDefault(s => s.Value.Equals(sensitivityTest.MinimumInhibitoryConcentration));

            if (selectedStep != null && Math.Abs(breakpoint.MicBreakpointSusceptible.Value - EucastExtraLowSusceptibleValueToAlwaysGetIntermediate) > 0.001f)
            {
                if (selectedStep.LowerBoundary && sensitivityTest.MinimumInhibitoryConcentration > breakpoint.MicBreakpointSusceptible
                || selectedStep.UpperBoundary && sensitivityTest.MinimumInhibitoryConcentration < breakpoint.MicBreakpointResistent)
                {
                    sensitivityTest.Resistance = Resistance.NotEvaluable;
                    return "bg-info";
                }
            }


            Logger.LogInformation($"Found breakpoint for {sensitivityTest.TestingMethod}/{sensitivityTest.AntifungalAgent} where id is {sensitivityTest.ClinicalBreakpointId}: {breakpoint.Title}");

            var mic = sensitivityTest.MinimumInhibitoryConcentration;
            if (IsResistantAccordingToEucastDefinition(mic, breakpoint))
            {
                sensitivityTest.Resistance = Resistance.Resistant;
                return "bg-danger";
            }
            if (IsResistantAccordingToClsiDefinition(mic, breakpoint))
            {
                sensitivityTest.Resistance = Resistance.Resistant;
                return "bg-danger";
            }
            if (IsSusceptibleAccordingToBothDefinitions(mic, breakpoint))
            {
                sensitivityTest.Resistance = Resistance.Susceptible;
                return "bg-success";
            }
            sensitivityTest.Resistance = Resistance.Intermediate;
            return "bg-warning";
        }
        
        internal bool CheckInternalNormalTypeVisibility()
        {
            var isInternalNormalUnit = SentinelEntry.HospitalDepartment == HospitalDepartment.Internal &&
                          SentinelEntry.HospitalDepartmentType == HospitalDepartmentType.NormalUnit;
            if (!isInternalNormalUnit)
            {
                SentinelEntry.InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment;
            }
            return isInternalNormalUnit;
        }

        private static bool IsResistantAccordingToEucastDefinition(float? mic, ClinicalBreakpoint breakpoint)
        {
            return mic > breakpoint.MicBreakpointResistent && breakpoint.Standard == BrothMicrodilutionStandard.Eucast;
        }

        private static bool IsResistantAccordingToClsiDefinition(float? mic, ClinicalBreakpoint breakpoint)
        {
            return mic >= breakpoint.MicBreakpointResistent && breakpoint.Standard == BrothMicrodilutionStandard.Clsi;
        }

        private static bool IsSusceptibleAccordingToBothDefinitions(float? mic, ClinicalBreakpoint breakpoint)
        {
            return mic <= breakpoint.MicBreakpointSusceptible;
        }

        internal async Task SubmitClick()
        {
            LastError = string.Empty;

            try
            {
                if (IsEdit())
                {
                    await SentinelEntryService.Update(SentinelEntry).ConfigureAwait(true);
                }
                else
                {
                    await SentinelEntryService.Create(SentinelEntry).ConfigureAwait(true);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Storing failed");
                LastError = e.Message;
            }

            if (!SaveFailed)
            {
                await BackToList().ConfigureAwait(true);
            }
        }

        protected async Task BackToList()
        {
            if (OnCloseSet)
            {
                await OnCloseClick.InvokeAsync(null).ConfigureAwait(true);
            }
            else
            {
                NavigationManager.NavigateTo("sentinel-entries", replace: true);
            }
        }

        internal IEnumerable<AntimicrobialSensitivityTestRequest> RecalculateResistance(Sub sub)
        {
            return sub.AntimicrobialSensitivityTests
                .OrderBy(a => a.TestingMethod)
                .ThenBy(a => a.AntifungalAgent, new AntifungalAgentComparer());
        }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation($"Now loading... /SentinelEntry/{(IsEdit()?"Edit":"Create")}");

            Title = IsEdit() ? "Bearbeiten" : "Neu anlegen";
            PrimaryAction = IsEdit() ? "Speichern" : "Anlegen";

            AllBreakpoints = await ClinicalBreakpointService.List().ConfigureAwait(true);

            if (Id.HasValue)
            {
                var response = await SentinelEntryService.GetById(Id.Value).ConfigureAwait(true);
                LaboratoryNumber = response.LaboratoryNumber;
                if (response.CryoDate.HasValue)
                {
                    CryoBox = response.CryoBox;
                }
                SentinelEntry = response;
            }
            else
            {
                SentinelEntry = new SentinelEntryRequest();
            }

            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        private bool IsEdit()
        {
            return Id.HasValue;
        }
    }
}
