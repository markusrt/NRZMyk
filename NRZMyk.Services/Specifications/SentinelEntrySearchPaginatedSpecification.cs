using System;
using System.Linq;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class SentinelEntrySearchPaginatedSpecification : Specification<SentinelEntry>
    {
        public string ProtectKey { get; }
        public string SearchTerm { get; }

        public SentinelEntrySearchPaginatedSpecification(int skip, int take, string protectKey, string searchTerm = null)
        {
            ProtectKey = protectKey;
            SearchTerm = searchTerm;
            
            var query = Query;

            // Filter by organization if specified
            if (!string.IsNullOrEmpty(protectKey))
            {
                query = query.Where(s => s.ProtectKey == protectKey);
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();
                query = query.Where(s => 
                    s.LaboratoryNumber.ToLower().Contains(searchTermLower) ||
                    s.SenderLaboratoryNumber.ToLower().Contains(searchTermLower) ||
                    (s.SamplingDate.HasValue && s.SamplingDate.Value.ToString("yyyy-MM-dd").Contains(searchTermLower)) ||
                    s.IdentifiedSpecies.ToString().ToLower().Contains(searchTermLower) ||
                    (!string.IsNullOrEmpty(s.OtherIdentifiedSpecies) && s.OtherIdentifiedSpecies.ToLower().Contains(searchTermLower)));
            }

            query.OrderByDescending(s => s.Id)
                 .Skip(skip)
                 .Take(take);
        }
    }
}