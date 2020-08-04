using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class ClinicalBreakpointFilterSpecification : BaseSpecification<ClinicalBreakpoint>
    {
        public ClinicalBreakpointFilterSpecification(Species? species)
            : base(i => !species.HasValue || i.Species == species)
        {
        }
    }
}
