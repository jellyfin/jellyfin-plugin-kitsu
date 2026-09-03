using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Jellyfin.Plugin.Anime.Providers.KitsuIO.ApiClient;
using Jellyfin.Plugin.Kitsu.Providers.KitsuIO.Services;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO.Metadata
{
    public class KitsuIoEpisodeProvider : IRemoteMetadataProvider<Episode, EpisodeInfo>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string Name => "Kitsu";

        public KitsuIoEpisodeProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(EpisodeInfo searchInfo, CancellationToken cancellationToken)
        {
            var id = searchInfo.ProviderIds.GetValueOrDefault("Kitsu");
            if (string.IsNullOrWhiteSpace(id))
            {
                return new List<RemoteSearchResult>();;
            }

            var apiResponse = await KitsuIoApi.Get_Episodes(id, _httpClientFactory);
            return apiResponse.Data.Select(x => new RemoteSearchResult
            {
                IndexNumber = x.Attributes.Number,
                Name = TitleSelector.GetTitle(x.Attributes),
                ParentIndexNumber = x.Attributes.SeasonNumber,
                PremiereDate = x.Attributes.AirDate,
                ProviderIds = new Dictionary<string, string> {{"Kitsu", x.Id.ToString()}},
                SearchProviderName = Name,
                Overview = x.Attributes.Synopsis,
            });
        }

        public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<Episode>();

            if (!info.SeriesProviderIds.TryGetValue("Kitsu", out var seriesId) || !info.IndexNumber.HasValue)
            {
                return result;
            }

            var episodeInfo = await KitsuIoApi.Get_Episode(seriesId, info.IndexNumber.Value, _httpClientFactory);
            if (episodeInfo?.Data?.Attributes == null)
            {
                return result;
            }

            result.HasMetadata = true;
            result.Item = new Episode
            {
                IndexNumber = info.IndexNumber,
                ParentIndexNumber = info.ParentIndexNumber ?? 1,
                Name = TitleSelector.GetTitle(episodeInfo.Data.Attributes),
                PremiereDate = episodeInfo.Data.Attributes.AirDate,
                Overview = episodeInfo.Data.Attributes.Synopsis,
                ProviderIds = new Dictionary<string, string>() { { "Kitsu", episodeInfo.Data.Id.ToString() } }
            };

            if (episodeInfo.Data.Attributes.Length != null)
            {
                result.Item.RunTimeTicks = TimeSpan.FromMinutes(episodeInfo.Data.Attributes.Length.Value).Ticks;
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);

            return await httpClient.GetAsync(url).ConfigureAwait(false);
        }
    }
}
