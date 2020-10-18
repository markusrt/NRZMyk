using System;
using NRZMyk.Server.ModelExtensions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;

namespace HaemophilusWeb.Tools
{
    public class SentinelEntryExportDefinition : ExportDefinition<SentinelEntry>
    {
        public SentinelEntryExportDefinition()
        {
            AddField(s => s.Id);
            AddField(s => s.LaboratoryNumber);
            AddField(s => s.CryoBox);
            AddField(s => ExportToString(s.SenderLaboratoryNumber));
            AddField(s => ToReportFormat(s.SamplingDate));
            AddField(s => ExportToString(s.AgeGroup));
            AddField(s => ExportToString(s.HospitalDepartmentType));
            AddField(s => ExportToString(s.HospitalDepartment));
            AddField(s => s.MaterialOrOther(), "Material");
            AddField(s => ExportToString(s.SpeciesIdentificationMethod));
            AddField(s => ExportToString(s.IdentifiedSpecies));
        }

        private static string ToReportFormat(DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : ToReportFormat(dateTime.Value);
        }

        private static string ToReportFormat(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }
    }
}