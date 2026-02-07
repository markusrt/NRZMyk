using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryByReceivingDateSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public SentinelEntryByReceivingDateSpecification(int take, string protectKey)
        {
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .OrderByDescending(s => s.ReceivingDate)
                .Take(take);
        }
    }
}
