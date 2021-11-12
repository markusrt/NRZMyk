using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;

namespace HaemophilusWeb.Tools
{
    public class AntimicrobialSensitivityTestExportDefinition : ExportDefinition<AntimicrobialSensitivityTest>
    {
        private readonly IMicStepsService _micStepsService;

        public AntimicrobialSensitivityTestExportDefinition(IMicStepsService micStepsService)
        {
            _micStepsService = micStepsService;
            AddField(m => m.SentinelEntry.Id, "Sentinel Datensatz Id");
            AddField(m => ExportToString(m.TestingMethod), "Test");
            AddField(m => ExportToString(m.AntifungalAgent), "Antimykotikum");
            AddField(m => GetMicValue(m), "MHK");
            AddField(m => ExportToString(m.Resistance), "Bewertung");
            AddField(m => GetClinicalBreakpointName(m), "Breakpoint");
        }

        private string GetMicValue(AntimicrobialSensitivityTest test)
        {
            var micStep = _micStepsService.StepsByTestingMethodAndAgent(test.TestingMethod, test.AntifungalAgent)
                .FirstOrDefault(s => s.Value.Equals(test.MinimumInhibitoryConcentration));
            return micStep != null
                ? micStep.Title
                : test.MinimumInhibitoryConcentration.ToString(CultureInfo.GetCultureInfo("de"));
        }

        private string GetClinicalBreakpointName(AntimicrobialSensitivityTest test)
        {
            var breakpoint = test.ClinicalBreakpoint;
            return breakpoint == null
                ? null //Kein Breakpoint für Antimykotikum "5-FC" und Spezies "Candida albicans" verfügbar.
                : $"{breakpoint.AntifungalAgentDetails} - {EnumUtils.GetEnumDescription(breakpoint.Species)} - v{breakpoint.Version}";;
        }
    }
}