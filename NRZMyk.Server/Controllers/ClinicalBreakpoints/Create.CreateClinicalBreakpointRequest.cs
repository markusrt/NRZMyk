using System;
using System.Text.Json.Serialization;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server.Controllers.ClinicalBreakpoints
{
    public class CreateClinicalBreakpointRequest : BaseRequest 
    {
        [JsonRequired] public AntifungalAgent  AntifungalAgent { get; set; }
        public string AntifungalAgentDetails { get; set; }
        [JsonRequired] public Species Species { get; set; }
        public BrothMicrodilutionStandard Standard { get; set; }
        public string Version { get; set; }
        [JsonRequired] public DateTime ValidFrom { get; set; }
        public float? MicBreakpointSusceptible { get; set; }
        public float? MicBreakpointResistent { get; set; }
    }
}
