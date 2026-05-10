using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public abstract class SentinelEntrySearchSpecificationBase : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }
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
