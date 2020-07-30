namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class ListPagedSentinelEntryRequest : BaseRequest 
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
