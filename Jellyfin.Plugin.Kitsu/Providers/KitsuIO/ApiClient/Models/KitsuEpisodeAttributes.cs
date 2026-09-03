using System;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models
{
    public class KitsuEpisodeAttributes : IKitsuAttributesWithTitles
    {
        public string Synopsis { get; set; }
        public string CanonicalTitle { get; set; }
        public KitsuTitles Titles { get; set; }
        public int? Number { get; set; }
        public int? SeasonNumber { get; set; }
        public DateTime? AirDate { get; set; }
        public int? Length { get; set; }
        public KitsuImage Thumbnail { get; set; }
    }
}
