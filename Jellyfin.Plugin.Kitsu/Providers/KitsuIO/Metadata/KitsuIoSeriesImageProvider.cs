using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Anime.Providers.KitsuIO.ApiClient;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.Metadata
{
    public class KitsuIoSeriesImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string Name => "Kitsu";

        public KitsuIoSeriesImageProvider(IHttpClientFactory httpClientFactory)
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

            if (!item.TryGetProviderId("Kitsu", out var seriesId) || string.IsNullOrWhiteSpace(seriesId))
            {
                return list;
            }

            var seriesInfo = await KitsuIoApi.Get_Series(seriesId, _httpClientFactory);

            var primaryImage = seriesInfo?.Data?.Attributes?.PosterImage?.Original?.ToString();
            if (!string.IsNullOrWhiteSpace(primaryImage))
            {
                list.Add(new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = primaryImage,
                    Type = ImageType.Primary
                });
            }

            var backdropImage = seriesInfo?.Data?.Attributes?.CoverImage?.Original?.ToString();
            if (!string.IsNullOrWhiteSpace(primaryImage))
            {
                list.Add(new RemoteImageInfo
                {
                    ProviderName = Name,
                    Url = backdropImage,
                    Type = ImageType.Backdrop
                });
            }

            return list;
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[]
            {
                ImageType.Primary,
                ImageType.Backdrop
            };
        }

        public bool Supports(BaseItem item)
        {
            return item is MediaBrowser.Controller.Entities.TV.Series;
        }
    }
}
