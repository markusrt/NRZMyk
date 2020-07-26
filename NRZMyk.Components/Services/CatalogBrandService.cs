using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using NRZMyk.Services.Models;

namespace NRZMyk.Components.Services
{
    public class CatalogBrandService
    {
        private readonly HttpClient _httpClient;

        public CatalogBrandService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CatalogBrandDto>> List()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CatalogBrandDto>>("api/catalog-brands");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return new List<CatalogBrandDto>();
        }

        public static string GetBrandName(IEnumerable<CatalogBrandDto> brands, int brandId)
        {
            var type = brands.FirstOrDefault(t => t.Id == brandId);

            return type == null ? "None" : type.Name;
        }

    }
}
