using System;
using System.Collections.Generic;

namespace NRZMyk.Server.Controllers.CatalogBrands
{
    public class ListCatalogBrandsResponse : BaseResponse
    {
        public ListCatalogBrandsResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ListCatalogBrandsResponse()
        {
        }

        public List<CatalogBrandDto> CatalogBrands { get; set; } = new List<CatalogBrandDto>();
    }
}
