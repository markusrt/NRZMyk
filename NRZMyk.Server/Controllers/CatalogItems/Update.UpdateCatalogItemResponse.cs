using System;

namespace NRZMyk.Server.Controllers.CatalogItems
{
    public class UpdateCatalogItemResponse : BaseResponse
    {
        public UpdateCatalogItemResponse(Guid correlationId) : base(correlationId)
        {
        }

        public UpdateCatalogItemResponse()
        {
        }

        public CatalogItemDto CatalogItem { get; set; }
    }
}
