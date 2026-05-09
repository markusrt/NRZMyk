using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntrySearchFilterSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }
        public string SearchTerm { get; }

        public SentinelEntrySearchFilterSpecification(string protectKey, string searchTerm = null)
        {
            ProtectKey = protectKey;
            SearchTerm = searchTerm;

            // Filter by organization if specified
            if (!string.IsNullOrEmpty(protectKey))
            {
                Query.Where(s => s.ProtectKey == protectKey);
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                Query.Where(s =>
                    s.SenderLaboratoryNumber.ToLower().Contains(searchTermLower) ||
                    (!string.IsNullOrEmpty(s.OtherIdentifiedSpecies) && s.OtherIdentifiedSpecies.ToLower().Contains(searchTermLower)));
            }

            Query.OrderByDescending(s => s.Id);
        }
    }
}