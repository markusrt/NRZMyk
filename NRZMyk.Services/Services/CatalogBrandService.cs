using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services
{
    public class CatalogBrandService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogBrandService> _logger;

        public CatalogBrandService(HttpClient httpClient, ILogger<CatalogBrandService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<CatalogBrandDto>> List()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CatalogBrandDto>>("api/catalog-brands");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to load catalog brands from backend");
                return new List<CatalogBrandDto> {new CatalogBrandDto {Id=-1, Name = "Failed to load brands"}};
            }
        }

        public static string GetBrandName(IEnumerable<CatalogBrandDto> brands, int brandId)
        {
            var type = brands.FirstOrDefault(t => t.Id == brandId);

            return type == null ? "None" : type.Name;
        }
    }
}
