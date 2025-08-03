using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Server.Controllers.SentinelEntries
{
    public class ListPagedSentinelEntryResponse
    {
        public List<SentinelEntry> SentinelEntries { get; set; } = new List<SentinelEntry>();
        public int PageCount { get; set; }
    }
}
