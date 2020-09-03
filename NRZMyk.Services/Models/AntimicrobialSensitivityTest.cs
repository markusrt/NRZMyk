using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Models
{
    public class AntimicrobialSensitivityTest
    {
        public SpeciesTestingMethod TestingMethod { get; set; }

        public ClinicalBreakpointReference EucastClinicalBreakpoint { get; set; }

        public float  MinimumInhibitoryConcentration  { get; set; }

        public Resistance Resistance { get; set; }
    }
}