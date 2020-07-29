using System;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class CreateSentinelEntryRequest : BaseRequest 
    {
        public DateTime? SamplingDate { get; set; }
        public string SenderLaboratoryNumber { get; set; }
        public Material Material { get; set; }
        public ResidentialTreatment ResidentialTreatment { get; set; }
        public string IdentifiedSpecies { get; set; }
        public SpeciesTestingMethod SpeciesTestingMethod { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public string Remark { get; set; }
    }

}
