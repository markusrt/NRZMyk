using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryByCryoDateSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public SentinelEntryByCryoDateSpecification(int take, string protectKey)
        {
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .OrderByDescending(s => s.CryoDate)
                .Take(take);
        }
    }
}
