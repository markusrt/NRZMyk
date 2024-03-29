﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Data.Entities
{
    public class ClinicalBreakpoint : BaseEntity, IAggregateRoot
    {
        public AntifungalAgent AntifungalAgent { get; set; }

        public string AntifungalAgentDetails { get; set; }
        
        public Species Species { get; set; }

        public BrothMicrodilutionStandard Standard { get; set; }
        
        public string Version { get; set; }

        public DateTime ValidFrom { get; set; }

        public float? MicBreakpointSusceptible { get; set; }

        public float? MicBreakpointResistent { get; set; }

        [JsonIgnore]
        public ICollection<AntimicrobialSensitivityTest> AntimicrobialSensitivityTests { get; set; }
        
        /// <summary>
        /// ATU or SSD for EUCAST or CSLI respectively
        /// </summary>
        public float? TechnicalUncertainty { get; set; }

        public string Title 
            => NotAvailable 
                ? $"{AntifungalAgentDetails} - {EnumUtils.GetEnumDescription(Species)} - ohne {Standard} Grenzwerte"
                : $"{AntifungalAgentDetails} - {EnumUtils.GetEnumDescription(Species)} - v{Version} vom {ValidFrom:dd. MMM. yy}";

        public bool NotAvailable
            => Version == null && MicBreakpointResistent == null && MicBreakpointSusceptible == null;
    }
}