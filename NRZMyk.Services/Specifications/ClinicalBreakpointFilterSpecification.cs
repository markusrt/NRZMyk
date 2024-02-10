using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class ClinicalBreakpointFilterSpecification : Specification<ClinicalBreakpoint>
    {
        public ClinicalBreakpointFilterSpecification(Species? species)
        {
            Query.Where(i => !species.HasValue || i.Species == species);
        }
    }
}
