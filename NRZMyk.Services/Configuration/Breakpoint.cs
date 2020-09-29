﻿using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Configuration
{
    public class Breakpoint
    {
        public Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>> MicSteps { get; set; }
    }
}