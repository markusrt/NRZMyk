using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorAdmin.Services.CatalogItemService;
using Newtonsoft.Json;

namespace NRZMyk.Components.Services.CatalogItemService
{
    public class ListPaged
    {
        private readonly HttpClient _httpClient;

        public ListPaged(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CatalogItem>> HandleAsync(int pageSize)
        {
            var catalogItems = new List<CatalogItem>();

            var result = (await _httpClient.GetAsync($"{Constants.API_URL}catalog-items?PageSize={pageSize}"));
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return catalogItems;
            }

            catalogItems = JsonConvert.DeserializeObject<PagedCatalogItemResult>(await result.Content.ReadAsStringAsync()).CatalogItems;

            return catalogItems;
        }

    }
}