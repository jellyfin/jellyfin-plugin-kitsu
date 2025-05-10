using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO
{
    public class KitsuIoExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item)
            => item is Series || item is Movie;

        public string ProviderName
            => "Kitsu";

        public string Key
            => "Kitsu";

        public ExternalIdMediaType? Type
            => ExternalIdMediaType.Series;
    }
}
