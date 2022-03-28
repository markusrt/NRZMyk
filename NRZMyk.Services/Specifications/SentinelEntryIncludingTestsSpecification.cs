using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryIncludingTestsSpecification : BaseSpecification<SentinelEntry>
    {
        public int Id { get; }

        public SentinelEntryIncludingTestsSpecification(int id) : base(b => b.Id == id)
        {
            Id = id;
            AddInclude(b => b.AntimicrobialSensitivityTests);
            AddInclude(b => b.PredecessorEntry);
            AddInclude($"{nameof(SentinelEntry.AntimicrobialSensitivityTests)}.{nameof(AntimicrobialSensitivityTest.ClinicalBreakpoint)}");
        }
    }
}
