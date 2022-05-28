using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Jellyfin.Plugin.Anime.Providers.KitsuIO.ApiClient;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO.Metadata
{
    public class KitsuIoSeriesProvider : IRemoteMetadataProvider<Series, SeriesInfo>, IHasOrder
    {
        private readonly ILogger<KitsuIoSeriesProvider> _log;
        private readonly IApplicationPaths _paths;
        private readonly IHttpClientFactory _httpClientFactory;
        public int Order => -4;
        public string Name => "Kitsu";

        public KitsuIoSeriesProvider(ILogger<KitsuIoSeriesProvider> logger, IApplicationPaths paths, IHttpClientFactory httpClientFactory)
        {
            _log = logger;
            _paths = paths;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(SeriesInfo searchInfo, CancellationToken cancellationToken)
        {
            var searchResults = new List<RemoteSearchResult>();

            if (searchInfo.TryGetProviderId("Kitsu", out var seriesId))
            {
                var series = await KitsuIoApi.Get_Series(seriesId, _httpClientFactory);
                if (series?.Data?.Attributes != null)
                {
                    searchResults.Add(MapToRemoteSearchResult(series.Data));
                }
            }

            var filters = GetFiltersFromSeriesInfo(searchInfo);
            if (filters.Any(x => !string.IsNullOrEmpty(x.Value)))
            {
                var searchResponse = await KitsuIoApi.Search_Series(filters, _httpClientFactory);
                var parsedSearchResponse = searchResponse?.Data?.Select(MapToRemoteSearchResult).ToList();
                if (parsedSearchResponse?.Any() ?? false)
                {
                    searchResults.AddRange(parsedSearchResponse);
                }
            }

            return searchResults;
        }

        public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Series>();

            var kitsuId = info.ProviderIds.GetValueOrDefault("Kitsu");
            if (string.IsNullOrWhiteSpace(kitsuId))
            {
                _log.LogInformation("Start KitsuIo... Searching({Name})", info.Name);
                var filters = GetFiltersFromSeriesInfo(info);
                var apiResponse = await KitsuIoApi.Search_Series(filters, _httpClientFactory);
                kitsuId = apiResponse.Data.FirstOrDefault(x => x.Attributes.Titles.Equal(info.Name))?.Id.ToString();
            }

            if (!string.IsNullOrEmpty(kitsuId))
            {
                var seriesInfo = await KitsuIoApi.Get_Series(kitsuId, _httpClientFactory);
                result.HasMetadata = true;
                result.Item = new Series
                {
                    Overview = seriesInfo.Data.Attributes.Synopsis,
                    // KitsuIO has a max rating of 100
                    CommunityRating = string.IsNullOrWhiteSpace(seriesInfo.Data.Attributes.AverageRating)
                        ? null
                        : MathF.Round(float.Parse(seriesInfo.Data.Attributes.AverageRating, System.Globalization.CultureInfo.InvariantCulture) / 10, 1),
                    ProviderIds = new Dictionary<string, string>() {{"Kitsu", kitsuId}},
                    Genres = seriesInfo.Included?.Select(x => x.Attributes.Name).ToArray()
                             ?? Array.Empty<string>()
                };

                StoreImageUrl(kitsuId, seriesInfo.Data.Attributes.PosterImage.Original.ToString(), "image");
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);

            return await httpClient.GetAsync(url).ConfigureAwait(false);
        }

        private Dictionary<string, string> GetFiltersFromSeriesInfo(SeriesInfo seriesInfo)
        {
            var filters = new Dictionary<string, string> {{"text", HttpUtility.UrlEncode(seriesInfo.Name)}};
            if(seriesInfo.Year.HasValue) filters.Add("seasonYear", HttpUtility.UrlEncode(seriesInfo.Year.ToString()));
            return filters;
        }

        private void StoreImageUrl(string series, string url, string type)
        {
            var path = Path.Combine(_paths.CachePath, "kitsu", type, series + ".txt");
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            File.WriteAllText(path, url);
        }

        private RemoteSearchResult MapToRemoteSearchResult(Series series)
        {
            var parsedSeries = new RemoteSearchResult
            {
                Name = series.Attributes.Titles.GetTitle,
                SearchProviderName = Name,
                ImageUrl = series.Attributes.PosterImage.Medium.ToString(),
                Overview = series.Attributes.Synopsis,
                ProductionYear = series.Attributes.StartDate?.Year,
                PremiereDate = series.Attributes.StartDate?.DateTime,
            };
            parsedSeries.SetProviderId("Kitsu", series.Id.ToString());

            return parsedSeries;
        }
    }
}
