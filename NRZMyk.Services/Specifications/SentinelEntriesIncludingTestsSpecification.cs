using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntriesIncludingTestsSpecification : BaseSpecification<SentinelEntry>
    {
        public SentinelEntriesIncludingTestsSpecification()
        {
            AddInclude(b => b.AntimicrobialSensitivityTests);
            AddInclude($"{nameof(SentinelEntry.AntimicrobialSensitivityTests)}.{nameof(AntimicrobialSensitivityTest.ClinicalBreakpoint)}");
        }
    }
}
