using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Anime.Providers.KitsuIO.ApiClient;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.Metadata
{
    public class KitsuIoEpisodeImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string Name => "Kitsu";

        public KitsuIoEpisodeImageProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);

            return await httpClient.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var list = new List<RemoteImageInfo>();

            if (!item.TryGetProviderId("Kitsu", out var episodeId) || string.IsNullOrWhiteSpace(episodeId))
            {
                return list;
            }

            var episodeInfo = await KitsuIoApi.Get_Episode(episodeId, _httpClientFactory);

            var primaryImage = episodeInfo?.Data?.Attributes?.Thumbnail?.Original?.ToString();
            if (!string.IsNullOrWhiteSpace(primaryImage))
            {
                list.Add(new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = primaryImage,
                    Type = ImageType.Primary
                });
            }

            return list;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[]
            {
                ImageType.Primary
            };
        }

        public bool Supports(BaseItem item)
        {
            return item is Episode;
        }
    }
}
