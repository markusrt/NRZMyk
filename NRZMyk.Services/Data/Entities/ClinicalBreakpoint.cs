using System;
using System.ComponentModel.DataAnnotations.Schema;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data.Entities
{
    public class ClinicalBreakpoint : BaseEntity, IAggregateRoot
    {
        public AntifungalAgent  AntifungalAgent { get; set; }

        public string AntifungalAgentDetails { get; set; }
        
        public Species Species { get; set; }
        
        public BrothMicrodilutionStandard Standard { get; set; }
        
        public string Version { get; set; }

        public DateTime ValidFrom { get; set; }

        public float? MicBreakpointSusceptible { get; set; }

        public float? MicBreakpointResistent { get; set; }

        public string Title 
            => NotAvailable 
                ? $"{AntifungalAgentDetails} - ohne {Standard} Grenzwerte"
                : $"{AntifungalAgentDetails} - v{Version} vom {ValidFrom:dd. MMM. yy}";

        public bool NotAvailable
            => Version == null && ValidFrom == null && MicBreakpointResistent == null && MicBreakpointSusceptible == null;
    }
}