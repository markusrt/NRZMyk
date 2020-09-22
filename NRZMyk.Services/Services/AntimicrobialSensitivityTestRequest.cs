using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services
{
    public class AntimicrobialSensitivityTestRequest
    {
        public SpeciesTestingMethod TestingMethod { get; set; }

        public AntifungalAgent AntifungalAgent { get; set; }

        public int? ClinicalBreakpointId { get; set; }

        public float MinimumInhibitoryConcentration { get; set; }

        public Resistance Resistance { get; set; }
        
        public BrothMicrodilutionStandard Standard { get; set; }
    }
}