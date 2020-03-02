using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OctopusSearch.Core
{
    public class OctopusRestClient
    {
        private HttpClient _client = new HttpClient();
        private readonly string _apiKey;

        public OctopusRestClient(string server, string apiKey)
        {
            _client.BaseAddress = new Uri(server);
            _apiKey = apiKey;
        }

        public async Task<List<T>> GetResources<T>(string resourceUrl)
        {
            var responseJson = await _client.GetStringAsync($"{_client.BaseAddress}api/{resourceUrl}?apikey={_apiKey}");

            return JsonConvert.DeserializeObject<List<T>>(responseJson);
        }

        public async Task<T> GetResource<T>(string resourceUrl)
        {
            var responseJson = await _client.GetStringAsync($"{_client.BaseAddress}api/{resourceUrl}?apikey={_apiKey}");

            return JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}
