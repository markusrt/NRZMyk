using System;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.ModelExtensions;
using NRZMyk.Services.Services;

namespace HaemophilusWeb.Tools
{
    public class SentinelEntryExportDefinition : ExportDefinition<SentinelEntry>
    {
        private readonly IProtectKeyToOrganizationResolver _organizationResolver;

        public SentinelEntryExportDefinition(IProtectKeyToOrganizationResolver organizationResolver)
        {
            _organizationResolver = organizationResolver;
            AddField(s => s.Id);
            AddField(s => s.LaboratoryNumber);
            AddField(s => s.CryoBox);
            AddField(s => ExportToString(s.SenderLaboratoryNumber));
            AddField(s => ToReportFormat(s.SamplingDate));
            AddField(s => ExportToString(s.AgeGroup));
            AddField(s => ExportToString(s.Gender));
            AddField(s => ExportToString(s.HospitalDepartmentType));
            AddField(s => s.HospitalDepartmentOrOther(), "Station");
            AddField(s => s.MaterialOrOther(), "Material");
            AddField(s => s.SpeciesIdentificationMethodWithPcrDetails(), "Methode Speziesidentifikation");
            AddField(s => s.SpeciesOrOther(), "Spezies");
            AddField(s => ResolveSender(s), "Einsender");
            AddField(s => s.PredecessorLaboratoryNumber, "Labornummer Vorgänger");
        }

        private static string ToReportFormat(DateTime? dateTime)
        {
            return !dateTime.HasValue ? string.Empty : ToReportFormat(dateTime.Value);
        }

        private static string ToReportFormat(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }
        
        private string ResolveSender(SentinelEntry sentinelEntry)
        {
            //TODO Fix use of async here
            var sender = _organizationResolver.ResolveOrganization(sentinelEntry.ProtectKey).Result;
            return string.IsNullOrEmpty(sender)
                ? "Unbekannt"
                : sender;
        }
    }
}