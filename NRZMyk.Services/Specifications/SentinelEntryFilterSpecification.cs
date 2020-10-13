using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class SentinelEntryFilterSpecification : BaseSpecification<SentinelEntry>
    {
        public string ProtectKey { get; set; }
        public SentinelEntryFilterSpecification(string protectKey) : base(s => s.ProtectKey == protectKey)
        {
            ProtectKey = protectKey;
        }
    }
}
