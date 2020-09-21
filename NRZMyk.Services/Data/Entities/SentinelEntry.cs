using System;
using System.Collections.Generic;
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

        [Display(Name = "Stationstyp")]
        public HospitalDepartmentType HospitalDepartmentType { get; set; }

        [Display(Name = "Station")]
        public HospitalDepartment HospitalDepartment { get; set; }

        [Display(Name = "Methode Speziesidentifikation")]
        public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }

        [Display(Name = "Spezies")]
        public Species IdentifiedSpecies { get; set; }

        [Display(Name = "Altersgruppe")]
        public AgeGroup AgeGroup { get; set; }

        [Display(Name = "Bemerkung")]
        public string Remark { get; set; }

        public ICollection<AntimicrobialSensitivityTest> AntimicrobialSensitivityTests { get; set; }
    }
}