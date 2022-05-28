using System;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models
{
    public class KitsuSeries
    {
        public long Id { get; set; }
        public KitsuSeriesAttributes Attributes { get; set; }
    }

    public class KitsuSeriesAttributes
    {
        public string Synopsis { get; set; }
        public KitsuTitles Titles { get; set; }
        public string AverageRating { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public KitsuImage PosterImage { get; set; }
        public KitsuImage CoverImage { get; set; }
    }
}
