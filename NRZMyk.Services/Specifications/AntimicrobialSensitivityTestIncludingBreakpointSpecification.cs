using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class AntimicrobialSensitivityTestIncludingBreakpointSpecification 
        : BaseSpecification<AntimicrobialSensitivityTest>
    {
        public AntimicrobialSensitivityTestIncludingBreakpointSpecification()
        {
            AddInclude(a => a.ClinicalBreakpoint);
        }
    }
}
