using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;

namespace Jellyfin.Plugin.Anime.Providers.KitsuIO;

public class KitsuExternalUrlProvider : IExternalUrlProvider
{
    public string Name => "Kitsu";

    public IEnumerable<string> GetExternalUrls(BaseItem item)
    {
        if (item.TryGetProviderId("Kitsu", out var externalId))
        {
            switch (item)
            {
                case Series:
                case Movie:
                    yield return $"https://kitsu.app/anime/{externalId}";
                    break;
            }
        }
    }
}
