using System;

namespace NRZMyk.Server.Controllers.CatalogItems
{
    public class GetByIdCatalogItemResponse : BaseResponse
    {
        public GetByIdCatalogItemResponse(Guid correlationId) : base(correlationId)
        {
        }

        public GetByIdCatalogItemResponse()
        {
        }

        public CatalogItemDto CatalogItem { get; set; }
    }
}
