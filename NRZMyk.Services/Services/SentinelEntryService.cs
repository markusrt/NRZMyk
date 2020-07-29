using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public class SentinelEntryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SentinelEntryService> _logger;

        public SentinelEntryService(HttpClient httpClient, ILogger<SentinelEntryService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<SentinelEntry>> ListPaged(int pageSize)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PagedSentinelEntryResult>($"api/sentinel-entries?PageSize={pageSize}");
                return response.SentinelEntries;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load paged sentinel entries from backend");
                return new List<SentinelEntry>();
            }
        }
    }

    public class PagedSentinelEntryResult
    {
        public List<SentinelEntry> SentinelEntries { get; set; } = new List<SentinelEntry>();
        public int PageCount { get; set; } = 0;
    }
}
