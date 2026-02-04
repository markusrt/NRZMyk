using System;
using System.Linq;
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
                    s.SenderLaboratoryNumber.ToLower().Contains(searchTermLower) ||
                    (!string.IsNullOrEmpty(s.OtherIdentifiedSpecies) && s.OtherIdentifiedSpecies.ToLower().Contains(searchTermLower)));
            }

            query = query.OrderByDescending(s => s.Id);
        }
    }
}