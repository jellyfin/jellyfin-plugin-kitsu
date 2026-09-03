using System;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models
{
    public class KitsuSeriesAttributes : IKitsuAttributesWithTitles
    {
        public string Synopsis { get; set; }
        public string CanonicalTitle { get; set; }
        public KitsuTitles Titles { get; set; }
        public string AverageRating { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public KitsuImage PosterImage { get; set; }
        public KitsuImage CoverImage { get; set; }
    }
}
