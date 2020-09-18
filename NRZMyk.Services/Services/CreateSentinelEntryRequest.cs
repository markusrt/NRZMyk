using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services
{
    public class CreateSentinelEntryRequest
    {
        public DateTime? SamplingDate { get; set; }

        [Required(ErrorMessage = "Das Feld Labornummer Einsender ist erforderlich")]
        public string SenderLaboratoryNumber { get; set; }

        public Material Material { get; set; }

        public HospitalDepartmentType HospitalDepartmentType { get; set; }

        public HospitalDepartment HospitalDepartment { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Das Feld Spezies ist erforderlich")]
        public Species IdentifiedSpecies { get; set; }

        public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }

        public AgeGroup AgeGroup { get; set; }

        public string Remark { get; set; }

        public IList<AntimicrobialSensitivityTest> SensitivityTests { get; } = new List<AntimicrobialSensitivityTest>();
    }
}