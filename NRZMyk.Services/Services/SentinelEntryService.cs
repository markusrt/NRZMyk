using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface SentinelEntryService
    {
        Task<SentinelEntry> Create(CreateSentinelEntryRequest createRequest);
        Task<List<SentinelEntry>> ListPaged(int pageSize);
    }

    public class SentinelEntryServiceImpl : SentinelEntryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SentinelEntryServiceImpl> _logger;

        public SentinelEntryServiceImpl(HttpClient httpClient, ILogger<SentinelEntryServiceImpl> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<SentinelEntry> Create(CreateSentinelEntryRequest createRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/sentinel-entries", createRequest);
                return await response.Content.ReadFromJsonAsync<SentinelEntry>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create sentinel entry");
                return new SentinelEntry();
            }
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
}