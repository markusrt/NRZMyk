using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications;

public sealed class SentinelEntryCountSpecification : Specification<SentinelEntry>
{
    public SentinelEntryCountSpecification(string protectKey, bool? hasCryoDate = null, int? year = null)
    {
        Query.Where(s => s.ProtectKey == protectKey);

        if (hasCryoDate.HasValue)
        {
            if (hasCryoDate.Value)
            {
                Query.Where(s => s.CryoDate != null);
            }
            else
            {
                Query.Where(s => s.CryoDate == null);
            }
        }

        if (year.HasValue)
        {
            Query.Where(s => s.Year == year.Value);
        }
    }
}
