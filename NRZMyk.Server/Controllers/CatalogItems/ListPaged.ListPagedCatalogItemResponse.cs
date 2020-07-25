using System;
using System.Collections.Generic;

namespace NRZMyk.Server.Controllers.CatalogItems
{
    public class ListPagedCatalogItemResponse : BaseResponse
    {
        public ListPagedCatalogItemResponse(Guid correlationId) : base(correlationId)
        {
        }

        public ListPagedCatalogItemResponse()
        {
        }

        public List<CatalogItemDto> CatalogItems { get; set; } = new List<CatalogItemDto>();
        public int PageCount { get; set; }
    }
}
