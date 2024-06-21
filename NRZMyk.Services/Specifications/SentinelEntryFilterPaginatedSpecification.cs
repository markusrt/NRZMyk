using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryFilterPaginatedSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public SentinelEntryFilterPaginatedSpecification(int skip, int take, string protectKey)
        {
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .OrderByDescending(s => s.Id)
                .Skip(skip).Take(take);
        }
    }
}
