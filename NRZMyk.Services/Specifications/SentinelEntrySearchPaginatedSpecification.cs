using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntrySearchPaginatedSpecification : SentinelEntrySearchSpecificationBase
    {
        public SentinelEntrySearchPaginatedSpecification(int skip, int take, string protectKey, string searchTerm = null) : base(protectKey, searchTerm)
        {
            OrderByNewest();
            Query.Skip(skip).Take(take);
        }
    }
}
