using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface IClinicalBreakpointService
    {
        Task<List<ClinicalBreakpoint>> List();
    }

    public class ClinicalBreakpointService : IClinicalBreakpointService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClinicalBreakpointService> _logger;

        public ClinicalBreakpointService(HttpClient httpClient, ILogger<ClinicalBreakpointService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ClinicalBreakpoint>> List()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ClinicalBreakpoint>>("api/clinical-breakpoints");
                _logger.LogInformation($"API returned {response.Count} clinical breakpoints");
                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load paged sentinel entries from backend");
                return new List<ClinicalBreakpoint>();
            }
        }
    }
}