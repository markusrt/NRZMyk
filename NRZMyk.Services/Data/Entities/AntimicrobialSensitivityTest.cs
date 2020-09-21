using System.Text.Json.Serialization;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Data.Entities
{
    public class AntimicrobialSensitivityTest : BaseEntity, IAggregateRoot
    {
        public SpeciesTestingMethod TestingMethod { get; set; }

        public AntifungalAgent AntifungalAgent { get; set; }

        [JsonIgnore]
        public SentinelEntry SentinelEntry { get; set; }

        public int ClinicalBreakpointId { get; set; }

        public ClinicalBreakpoint ClinicalBreakpoint { get; set; }

        public float MinimumInhibitoryConcentration { get; set; }

        public Resistance Resistance { get; set; }
    }
}