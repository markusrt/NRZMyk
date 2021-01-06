using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.Utils;

namespace HaemophilusWeb.Tools
{
    public class AntimicrobialSensitivityTestExportDefinition : ExportDefinition<AntimicrobialSensitivityTest>
    {
        public AntimicrobialSensitivityTestExportDefinition()
        {
            AddField(m => m.SentinelEntry.Id, "Sentinel Datensatz Id");
            AddField(m => ExportToString(m.TestingMethod), "Test");
            AddField(m => ExportToString(m.AntifungalAgent), "Antimykotikum");
            AddField(m => m.MinimumInhibitoryConcentration, "MHK");
            AddField(m => ExportToString(m.Resistance), "Bewertung");
            AddField(m => GetClinicalBreakpointName(m), "Breakpoint");
        }

        private string GetClinicalBreakpointName(AntimicrobialSensitivityTest test)
        {
            var breakpoint = test.ClinicalBreakpoint;
            return breakpoint == null
                ? null
                : $"{breakpoint.AntifungalAgentDetails} - {EnumUtils.GetEnumDescription(breakpoint.Species)} - v{breakpoint.Version}";;
        }
    }
}