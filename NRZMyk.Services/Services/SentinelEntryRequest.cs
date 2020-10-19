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
        private static Func<SentinelEntryRequest, int> Value = request => (int) request.Material;

        public int Id { get; set; }

        public DateTime? SamplingDate { get; set; }

        [Required(ErrorMessage = "Das Feld Labornummer Einsender ist erforderlich")]
        public string SenderLaboratoryNumber { get; set; }

        public Material Material { get; set; }

        [OtherValue((int) Material.Other, nameof(Material))]
        public string OtherMaterial { get; set; }

        public HospitalDepartmentType HospitalDepartmentType { get; set; }

        public HospitalDepartment HospitalDepartment { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Spezies ist erforderlich")]
        public Species IdentifiedSpecies { get; set; }

        [OtherValue((int) Material.Other, nameof(IdentifiedSpecies))]
        public string OtherIdentifiedSpecies { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Methode Speziesidentifikation ist erforderlich")]
        public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }

        public AgeGroup AgeGroup { get; set; }

        
        public string Remark { get; set; }

        public Gender Gender { get; set; }
        
        public List<AntimicrobialSensitivityTestRequest> AntimicrobialSensitivityTests { get; set;} = new List<AntimicrobialSensitivityTestRequest>();

        private static int GetMaterial(SentinelEntryRequest sentinelEntryRequest)
        {
            return (int) sentinelEntryRequest.Material;
        }
    }
}