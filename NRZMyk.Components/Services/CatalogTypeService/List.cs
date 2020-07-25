using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json;

namespace NRZMyk.Components.Services.CatalogTypeService
{
    public class List
    {
        private readonly HttpClient _httpClient;

        public List(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CatalogType>> HandleAsync()
        {
            var types = new List<CatalogType>();

            //if (!_authService.IsLoggedIn)
            //{
            //    return types;
            //}

            try
            {
                var result = (await _httpClient.GetAsync($"{Constants.API_URL}catalog-types"));
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    return types;
                }

                types = JsonConvert.DeserializeObject<CatalogTypeResult>(await result.Content.ReadAsStringAsync()).CatalogTypes;
            }
            catch (AccessTokenNotAvailableException)
            {
                return types;
            }

            return types;
        }

        public static string GetTypeName(IEnumerable<CatalogType> types, int typeId)
        {
            var type = types.FirstOrDefault(t => t.Id == typeId);

            return type == null ? "None" : type.Name;
        }

    }
}
