using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryIncludingTestsSpecification : Specification<SentinelEntry>
    {
        public int Id { get; }

        public SentinelEntryIncludingTestsSpecification(int id)
        {
            Id = id;
            Query
                .Where(b => b.Id == id)
                .Include(b => b.PredecessorEntry)
                .Include(b => b.AntimicrobialSensitivityTests)
                .ThenInclude(s => s.ClinicalBreakpoint);
        }
    }
}
