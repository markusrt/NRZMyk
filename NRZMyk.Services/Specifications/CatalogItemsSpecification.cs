using System.Linq;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class CatalogItemsSpecification : BaseSpecification<CatalogItem>
    {
        public CatalogItemsSpecification(params int[] ids) : base(c => ids.Contains(c.Id))
        {

        }
    }
}
