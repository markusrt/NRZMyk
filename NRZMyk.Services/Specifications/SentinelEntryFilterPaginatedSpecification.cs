using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class SentinelEntryFilterPaginatedSpecification : SentinelEntryFilterSpecification
    {
        public SentinelEntryFilterPaginatedSpecification(int skip, int take, string protectKey) : base(protectKey)
        {
            ApplyPaging(skip, take);
            ApplyOrderByDescending(s => s.Id);
        }
    }
}
