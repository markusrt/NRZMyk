using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public class PagedSentinelEntryResult
    {
        public List<SentinelEntry> SentinelEntries { get; set; } = new List<SentinelEntry>();
        public int PageCount { get; set; } = 0;
    }
}
