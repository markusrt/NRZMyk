using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using NRZMyk.Components.Model;

namespace NRZMyk.Components.Services
{
    public class CatalogBrandService
    {
        private readonly HttpClient _httpClient;

        public CatalogBrandService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CatalogBrand>> List()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CatalogBrand>>("api/catalog-brands");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
            return new List<CatalogBrand>();
        }

        public static string GetBrandName(IEnumerable<CatalogBrand> brands, int brandId)
        {
            var type = brands.FirstOrDefault(t => t.Id == brandId);

            return type == null ? "None" : type.Name;
        }

    }
}
