using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
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

    public class CreateSentinelEntryRequest
    {
        public DateTime? SamplingDate { get; set; }

        [Required(ErrorMessage = "Das Feld Labornummer Einsender ist erforderlich")]
        public string SenderLaboratoryNumber { get; set; }

        public Material Material { get; set; }

        public ResidentialTreatment ResidentialTreatment { get; set; }

        [Required(ErrorMessage = "Das Feld Spezies ist erforderlich")]
        public string IdentifiedSpecies { get; set; }

        public SpeciesTestingMethod SpeciesTestingMethod { get; set; }

        public AgeGroup AgeGroup { get; set; }

        public string Remark { get; set; }
    }

    public class PagedSentinelEntryResult
    {
        public List<SentinelEntry> SentinelEntries { get; set; } = new List<SentinelEntry>();
        public int PageCount { get; set; } = 0;
    }
}
