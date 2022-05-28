using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models
{
    public class KitsuTitles
    {
        [JsonPropertyName("en")] public string En { get; set; }

        [JsonPropertyName("en_jp")] public string EnJp { get; set; }

        [JsonPropertyName("ja_jp")] public string JaJp { get; set; }

        [JsonPropertyName("en_us")] public string EnUs { get; set; }

        public string GetTitle =>
            !string.IsNullOrWhiteSpace(En) ? En :
            !string.IsNullOrWhiteSpace(EnUs) ? EnUs :
            !string.IsNullOrWhiteSpace(EnJp) ? EnJp :
            JaJp;

        public bool Equal(string title)
        {
            return
                (En?.Equals(title) ?? false) ||
                (EnUs?.Equals(title) ?? false) ||
                (EnJp?.Equals(title) ?? false) ||
                (JaJp?.Equals(title) ?? false);
        }
    }
}
