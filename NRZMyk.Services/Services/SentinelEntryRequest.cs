using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Validation;

namespace NRZMyk.Services.Services
{
    public class SentinelEntryRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Das Feld \"Probenentnahme\" ist erforderlich")]
        public DateTime? SamplingDate { get; set; }

        [Required(ErrorMessage = "Das Feld Labornummer Einsender ist erforderlich")]
        public string SenderLaboratoryNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Material ist erforderlich")]
        public Material Material { get; set; }

        [OtherValue((int) Material.Other, nameof(Material), ErrorMessage = "Das Feld Anderes Material ist erforderlich")]
        public string OtherMaterial { get; set; }

        public HospitalDepartmentType HospitalDepartmentType { get; set; }

        public HospitalDepartment HospitalDepartment { get; set; }
        
        [OtherValue((int) HospitalDepartment.Other, nameof(HospitalDepartment), ErrorMessage = "Das Feld Andere Abteilung ist erforderlich")]
        public string OtherHospitalDepartment { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Spezies ist erforderlich")]
        public Species IdentifiedSpecies { get; set; }

        [OtherValue((int) Material.Other, nameof(IdentifiedSpecies), ErrorMessage = "Das Feld Andere Spezies ist erforderlich")]
        public string OtherIdentifiedSpecies { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Methode Speziesidentifikation ist erforderlich")]
        public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }
        
        [OtherValue((int) SpeciesIdentificationMethod.Pcr, nameof(SpeciesIdentificationMethod), ErrorMessage = "Das Feld PCR Details ist erforderlich")]
        public string PcrDetails { get; set; }

        public AgeGroup AgeGroup { get; set; }
        
        public string Remark { get; set; }

        public Gender Gender { get; set; }
        
        
        [SensitivityTestNotEmptyWithoutComment(
            ErrorMessage = "Mindestens ein MHK Eintrag ist erforderlich. Schreiben sie bitte einen Erklärung in die Anmerkungen, falls es keine MHKs ermitteln konnten." )]
        [SensitivityTest(ErrorMessage = "Bitte tragen sie für jeden MHK einen Messwert ein")]
        public List<AntimicrobialSensitivityTestRequest> AntimicrobialSensitivityTests { get; set;} = new List<AntimicrobialSensitivityTestRequest>();

        private static int GetMaterial(SentinelEntryRequest sentinelEntryRequest)
        {
            return (int) sentinelEntryRequest.Material;
        }
    }
}