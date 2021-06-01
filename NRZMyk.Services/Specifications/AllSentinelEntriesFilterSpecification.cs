using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class AllSentinelEntriesFilterSpecification : BaseSpecification<SentinelEntry>
    {
        public AllSentinelEntriesFilterSpecification()
        {
            ApplyOrderByDescending(s => s.Id);
        }
    }
}
