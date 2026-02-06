using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Extensions;
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

        public EditContext EditContext { get; private set; } = default!;
        
        private ValidationMessageStore _validationMessageStore = default!;

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
            if (sensitivityTest.MinimumInhibitoryConcentration.HasValue && matchingSteps.Any() && matchingSteps.TrueForAll(s => !s.Value.Equals(sensitivityTest.MinimumInhibitoryConcentration)))
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
                && b.Standard == sensitivityTest.Standard).OrderByDescending(b=>b.Version).ToList();
            
            if (applicableBreakpoints.All(b => b.Id != sensitivityTest.ClinicalBreakpointId))
            {
                Logger.LogInformation("Update test {sensitivityTest} to breakpoint id {breakpoint}",
                    sensitivityTest.AntifungalAgent, applicableBreakpoints.FirstOrDefault()?.Id);
                sensitivityTest.ClinicalBreakpointId = applicableBreakpoints.FirstOrDefault()?.Id;
            }

            Logger.LogInformation("Found {NumberOfApplicableBreakpoints} applicable breakpoints for {AntifungalAgent} and {IdentifiedSpecies}",
                applicableBreakpoints.Count, antifungalAgent, SentinelEntry.IdentifiedSpecies);
            
            if (!applicableBreakpoints.Any())
            {
                sensitivityTest.Resistance = Resistance.NotDetermined;
            }
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

            var mic = MicStepsService.FloorToClosestReferenceValue(sensitivityTest.MinimumInhibitoryConcentration);
            
            if (mic.IsResistantAccordingToEucastDefinition(breakpoint))
            {
                sensitivityTest.Resistance = Resistance.Resistant;
                return "bg-danger";
            }
            if (mic.IsResistantAccordingToClsiDefinition(breakpoint))
            {
                sensitivityTest.Resistance = Resistance.Resistant;
                return "bg-danger";
            }
            if (mic.IsSusceptibleAccordingToBothDefinitions(breakpoint))
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

       

        internal async Task SubmitClick()
        {
            LastError = string.Empty;
            _validationMessageStore.Clear();

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
            catch (ServerValidationException validationException)
            {
                Logger.LogError(validationException, "Server validation failed");
                foreach (var error in validationException.ValidationErrors)
                {
                    var fieldIdentifier = new FieldIdentifier(SentinelEntry, error.Key);
                    foreach (var message in error.Value)
                    {
                        _validationMessageStore.Add(fieldIdentifier, message);
                    }
                }
                EditContext.NotifyValidationStateChanged();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Storing failed");
                LastError = e.Message;
            }

            if (!SaveFailed && !EditContext.GetValidationMessages().Any())
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

        internal IEnumerable<AntimicrobialSensitivityTestRequest> RecalculateResistance()
        {
            return SentinelEntry.AntimicrobialSensitivityTests
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

            EditContext = new EditContext(SentinelEntry);
            _validationMessageStore = new ValidationMessageStore(EditContext);

            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        private bool IsEdit()
        {
            return Id.HasValue;
        }
    }
}
