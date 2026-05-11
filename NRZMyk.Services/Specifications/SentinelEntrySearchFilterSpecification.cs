using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntrySearchFilterSpecification : SentinelEntrySearchSpecificationBase
    {
        public SentinelEntrySearchFilterSpecification(string protectKey, string searchTerm = null) : base(protectKey, searchTerm)
        {
            OrderByNewest();
        }
    }
}
