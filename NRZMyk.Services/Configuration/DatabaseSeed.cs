using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Configuration
{
    public class DatabaseSeed
    {
        public string MainOrganization { get; set; }
        public List<ClinicalBreakpoint> ClinicalBreakpoints { get; set; }
    }
}
