using System;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Export;
using NRZMyk.Services.ModelExtensions;
using NRZMyk.Services.Services;

namespace HaemophilusWeb.Tools
{
    public class SentinelEntryWithPredecessorExportDefinition : SentinelEntryExportDefinition
    {

        public SentinelEntryWithPredecessorExportDefinition(IProtectKeyToOrganizationResolver organizationResolver) : base(organizationResolver)
        {
            AddField(s => s.PredecessorLaboratoryNumber, "SN-Labornummer Vorgänger");
        }
    }
}