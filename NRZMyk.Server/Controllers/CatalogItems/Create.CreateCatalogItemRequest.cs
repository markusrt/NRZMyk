﻿namespace NRZMyk.Server.Controllers.CatalogItems
{
    public class CreateCatalogItemRequest : BaseRequest 
    {
        public int CatalogBrandId { get; set; }
        public int CatalogTypeId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PictureUri { get; set; }
        public decimal Price { get; set; }
    }

}
