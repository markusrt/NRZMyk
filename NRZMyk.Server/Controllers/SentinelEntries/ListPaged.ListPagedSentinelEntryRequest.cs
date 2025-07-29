using System.Text.Json.Serialization;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class ListPagedSentinelEntryRequest : BaseRequest 
    {
        [JsonRequired] public int PageSize { get; set; }
        [JsonRequired] public int PageIndex { get; set; }
        public string SearchTerm { get; set; }
        public int? OrganizationId { get; set; }
    }
}
