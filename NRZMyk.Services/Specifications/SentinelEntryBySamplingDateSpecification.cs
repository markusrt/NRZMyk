using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntryBySamplingDateSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }

        public SentinelEntryBySamplingDateSpecification(int take, string protectKey)
        {
            ProtectKey = protectKey;
            Query
                .Where(s => s.ProtectKey == protectKey)
                .OrderByDescending(s => s.SamplingDate)
                .Take(take);
        }
    }
}
