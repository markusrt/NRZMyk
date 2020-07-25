namespace NRZMyk.Server.Controllers.CatalogItems
{
    public class DeleteCatalogItemRequest : BaseRequest 
    {
        //[FromRoute]
        public int CatalogItemId { get; set; }
    }
}
