using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface SentinelEntryService
    {
        Task<SentinelEntry> Create(SentinelEntryRequest request);
        Task<List<SentinelEntry>> ListPaged(int pageSize);
        Task<SentinelEntryRequest> GetById(int id);
        Task<SentinelEntry> Update(SentinelEntryRequest updateRequest);
        Task<string> Export();
        Task<List<string>> Other(string other);
        Task Delete(int id);
    }

    public class SentinelEntryServiceImpl : SentinelEntryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ILogger<SentinelEntryServiceImpl> _logger;

        public SentinelEntryServiceImpl(HttpClient httpClient, IMapper mapper, ILogger<SentinelEntryServiceImpl> logger)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SentinelEntry> Create(SentinelEntryRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/sentinel-entries", request);
                return await response.Content.ReadFromJsonAsync<SentinelEntry>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to create sentinel entry");
                throw;
            }
        }

        public async Task<SentinelEntry> Update(SentinelEntryRequest updateRequest)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/sentinel-entries", updateRequest);
                return await response.Content.ReadFromJsonAsync<SentinelEntry>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to update sentinel entry");
                throw;
            }
        }

        public async Task<string> Export()
        {
            try
            {
                var data = await _httpClient.GetByteArrayAsync("api/sentinel-entries/export");
                return Convert.ToBase64String(data);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to export sentinel entries from backend");
                return string.Empty;
            }
        }

        public async Task<List<string>> Other(string other)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<string>>($"api/sentinel-entries/other/{other}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to load other {other} from backend");
                return new List<string>();
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/sentinel-entries/{id}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to retrieve sentinel entry");
                throw;
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

        public async Task<SentinelEntryRequest> GetById(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SentinelEntry>($"api/sentinel-entries/{id}");
                return _mapper.Map<SentinelEntryRequest>(response);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to retrieve sentinel entry");
                throw;
            }
        }
    }
}