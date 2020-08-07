using System;
using System.ComponentModel.DataAnnotations;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public class CreateSentinelEntryRequest
    {
        public DateTime? SamplingDate { get; set; }

        [Required(ErrorMessage = "Das Feld Labornummer Einsender ist erforderlich")]
        public string SenderLaboratoryNumber { get; set; }

        public Material Material { get; set; }

        public ResidentialTreatment ResidentialTreatment { get; set; }

        [Required(ErrorMessage = "Das Feld Spezies ist erforderlich")]
        public string IdentifiedSpecies { get; set; }

        public SpeciesTestingMethod SpeciesTestingMethod { get; set; }

        public AgeGroup AgeGroup { get; set; }

        public int ClinicalBreakpointId { get; set; }

        public string Remark { get; set; }
    }
}