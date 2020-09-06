using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface ClinicalBreakpointService
    {
        Task<List<ClinicalBreakpoint>> List();
    }

    public class ClinicalBreakpointServiceImpl : ClinicalBreakpointService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClinicalBreakpointServiceImpl> _logger;

        public ClinicalBreakpointServiceImpl(HttpClient httpClient, ILogger<ClinicalBreakpointServiceImpl> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ClinicalBreakpoint>> List()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ClinicalBreakpoint>>("api/clinical-breakpoints");
                foreach (var clinicalBreakpoint in response)
                {
                    _logger.LogInformation(clinicalBreakpoint.Title);
                }
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