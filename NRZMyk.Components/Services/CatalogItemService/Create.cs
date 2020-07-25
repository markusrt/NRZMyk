using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;
using NRZMyk.Services.Models;

namespace NRZMyk.Components.Services.CatalogItemService
{
    public class Create
    {
        private readonly HttpClient _httpClient;

        public Create(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CatalogItem> HandleAsync(CreateCatalogItemRequest catalogItem)
        {
            //try
            //{
            //    var forecasts = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
            //    Console.WriteLine(forecasts);
            //}
            //catch (AccessTokenNotAvailableException exception)
            //{
            //    exception.Redirect();
            //}

            var catalogItemResult = new CatalogItem();

            var content = new StringContent(JsonConvert.SerializeObject(catalogItem), Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync($"{Constants.API_URL}catalog-items", content);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return catalogItemResult;
            }

            catalogItemResult = JsonConvert.DeserializeObject<CreateCatalogItemResult>(await result.Content.ReadAsStringAsync()).CatalogItem;

            return catalogItemResult;
        }
    }
}
