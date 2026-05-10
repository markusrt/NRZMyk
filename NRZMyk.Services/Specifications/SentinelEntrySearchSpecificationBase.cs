using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    /// <summary>
    /// Shared base for sentinel entry search specifications with common organization and text filtering.
    /// </summary>
    public abstract class SentinelEntrySearchSpecificationBase : Specification<SentinelEntry>
    {
        /// <summary>
        /// Organization protect key used to constrain results. Null/empty means no organization filter.
        /// </summary>
        public string ProtectKey { get; }
        /// <summary>
        /// Optional free-text search term applied to SenderLaboratoryNumber and OtherIdentifiedSpecies.
        /// </summary>
        public string SearchTerm { get; }

        protected SentinelEntrySearchSpecificationBase(string protectKey, string searchTerm = null)
        {
            ProtectKey = protectKey;
            SearchTerm = searchTerm;

            if (!string.IsNullOrEmpty(protectKey))
            {
                Query.Where(s => s.ProtectKey == protectKey);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                Query.Where(s =>
                    s.SenderLaboratoryNumber.ToLower().Contains(searchTermLower) ||
                    (!string.IsNullOrEmpty(s.OtherIdentifiedSpecies) && s.OtherIdentifiedSpecies.ToLower().Contains(searchTermLower)));
            }
        }

        protected void OrderByNewest()
        {
            Query.OrderByDescending(s => s.Id);
        }
    }
}
