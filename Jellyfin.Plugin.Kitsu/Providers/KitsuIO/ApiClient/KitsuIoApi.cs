using Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models;
using MediaBrowser.Common.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO.ApiClient
{
    internal class KitsuIoApi
    {
        private const string _apiBaseUrl = "https://kitsu.io/api/edge";
        private static readonly JsonSerializerOptions _serializerOptions;

        static KitsuIoApi()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            _serializerOptions.Converters.Add(new LongToStringConverter());
            _serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public static async Task<ApiResponse<List<KitsuSeries>>> Search_Series(Dictionary<string, string> filters, IHttpClientFactory httpClientFactory)
        {
            var filterString = string.Join("&",filters.Select(x => $"filter[{x.Key}]={x.Value}"));
            var pageString = "page[limit]=10";

            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            var responseStream = await httpClient.GetStreamAsync($"{_apiBaseUrl}/anime?{filterString}&{pageString}");
            return await JsonSerializer.DeserializeAsync<ApiResponse<List<KitsuSeries>>>(responseStream, _serializerOptions);
        }

        public static async Task<ApiResponse<KitsuSeries>> Get_Series(string seriesId, IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            var responseStream = await httpClient.GetStreamAsync($"{_apiBaseUrl}/anime/{seriesId}?include=genres");
            return await JsonSerializer.DeserializeAsync<ApiResponse<KitsuSeries>>(responseStream, _serializerOptions);
        }

        public static async Task<ApiResponse<List<KitsuEpisode>>> Get_Episodes(string seriesId, IHttpClientFactory httpClientFactory)
        {
            var result = new ApiResponse<List<KitsuEpisode>>(new List<KitsuEpisode>());
            long episodeCount = 10;
            var step = 10;

            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            for (long offset = 0; offset < episodeCount; offset += step)
            {
                var queryString = $"?filter[mediaId]={seriesId}&page[limit]={step}&page[offset]={offset}";
                var responseStream = await httpClient.GetStreamAsync($"{_apiBaseUrl}/episodes{queryString}");
                var response = await JsonSerializer.DeserializeAsync<ApiResponse<List<KitsuEpisode>>>(responseStream, _serializerOptions);

                episodeCount = response.Meta.Count.Value;
                result.Data.AddRange(response.Data);
            }

            return result;
        }

        public static async Task<ApiResponse<KitsuEpisode>> Get_Episode(string episodeId, IHttpClientFactory httpClientFactory)
        {
            var filterString = $"/{episodeId}";

            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            var responseStream = await httpClient.GetStreamAsync($"{_apiBaseUrl}/episodes{filterString}");
            return await JsonSerializer.DeserializeAsync<ApiResponse<KitsuEpisode>>(responseStream, _serializerOptions);
        }

        public static async Task<ApiResponse<KitsuEpisode>> Get_Episode(string seriesId, int episodeNumber, IHttpClientFactory httpClientFactory)
        {
            var queryString = $"?filter[mediaId]={seriesId}&filter[number]={episodeNumber}";

            var httpClient = httpClientFactory.CreateClient(NamedClient.Default);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            var responseStream = await httpClient.GetStreamAsync($"{_apiBaseUrl}/episodes{queryString}");
            var response = await JsonSerializer.DeserializeAsync<ApiResponse<List<KitsuEpisode>>>(responseStream, _serializerOptions);

            return new ApiResponse<KitsuEpisode>
            {
                Data = response.Data.FirstOrDefault(),
                Included = response.Included,
                Meta = response.Meta
            };
        }
    }
}
