using System;
using System.ComponentModel.DataAnnotations;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data.Entities
{
    public class SentinelEntry : BaseEntity, IAggregateRoot
    {
        [Display(Name = "Entnahmedatum")]
        public DateTime? SamplingDate { get; set; }

        [Display(Name = "Labnr. Einsender")]
        public string SenderLaboratoryNumber { get; set; }
        
        [Display(Name = "Material")]
        public Material Material { get; set; }

        [Display(Name = "Art der stationären Betreuung")]
        public ResidentialTreatment ResidentialTreatment { get; set; }

        [Display(Name = "Speziesidentifizierung")]
        public string IdentifiedSpecies { get; set; }

        [Display(Name = "Methodik der Testung")]
        public SpeciesTestingMethod SpeciesTestingMethod { get; set; }

        [Display(Name = "Altersgruppe")]
        public AgeGroup AgeGroup { get; set; }

        [Display(Name = "Bemerkung")]
        public string Remark { get; set; }
    }
}