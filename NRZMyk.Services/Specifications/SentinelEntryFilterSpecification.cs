using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryFilterSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public SentinelEntryFilterSpecification(string protectKey)
        {
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .OrderByDescending(s => s.Id);
        }
    }
}
