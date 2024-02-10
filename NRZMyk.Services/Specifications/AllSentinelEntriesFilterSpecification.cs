using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class AllSentinelEntriesFilterSpecification : Specification<SentinelEntry>
    {
        public AllSentinelEntriesFilterSpecification()
        {
            Query.OrderByDescending(s => s.Id);
        }
    }
}
