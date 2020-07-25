using System;
using System.Collections.Generic;

namespace NRZMyk.Server.Controllers.CatalogTypes
{
    public class ListCatalogTypesResponse : BaseResponse
    {
        public ListCatalogTypesResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ListCatalogTypesResponse()
        {
        }

        public List<CatalogTypeDto> CatalogTypes { get; set; } = new List<CatalogTypeDto>();
    }
}
