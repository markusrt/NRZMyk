using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntriesIncludingTestsSpecification : Specification<SentinelEntry>
    {
        public SentinelEntriesIncludingTestsSpecification()
        {
            Query
                .Include(b => b.AntimicrobialSensitivityTests)
                .ThenInclude(s => s.ClinicalBreakpoint);
        }
    }
}
