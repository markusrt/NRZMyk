using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class SentinelEntryFilterPaginatedSpecification : BaseSpecification<SentinelEntry>
    {
        public SentinelEntryFilterPaginatedSpecification(int skip, int take)
        {
            ApplyPaging(skip, take);
        }
    }
}
