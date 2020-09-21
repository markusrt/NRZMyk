using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class SentinelEntryFilterPaginatedSpecification : SentinelEntryFilterSpecification
    {
        public SentinelEntryFilterPaginatedSpecification(int skip, int take)
        {
            ApplyPaging(skip, take);
        }
    }
}
