using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.Kitsu.Configuration;
using Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.Services;

public static class TitleSelector
{
    public static string GetOriginalTitle(IKitsuAttributesWithTitles attributes)
        => GetFirstTitleByPreference(attributes, Plugin.Instance.Configuration.OriginalTitlePreference);

    public static string GetTitle(IKitsuAttributesWithTitles attributes)
        => GetFirstTitleByPreference(attributes, Plugin.Instance.Configuration.TitlePreference);

    public static ICollection<string?> GetTitles(IKitsuAttributesWithTitles attributes)
        => GetTitlesOrderedByPreference(attributes, Plugin.Instance.Configuration.TitlePreference);

    static string GetFirstTitleByPreference(IKitsuAttributesWithTitles attribute, TitlePreferenceType preference) =>
        GetTitlesOrderedByPreference(attribute, preference).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? string.Empty;

    static ICollection<string?> GetTitlesOrderedByPreference(IKitsuAttributesWithTitles attribute, TitlePreferenceType preference) =>
        preference switch
        {
            TitlePreferenceType.English => [attribute.Titles.En, attribute.Titles.EnUs, attribute.CanonicalTitle, attribute.Titles.EnJp, attribute.Titles.JaJp],
            TitlePreferenceType.Japanese => [attribute.Titles.JaJp, attribute.Titles.EnJp, attribute.CanonicalTitle, attribute.Titles.En, attribute.Titles.EnUs,],
            TitlePreferenceType.JapaneseRomaji => [attribute.Titles.EnJp, attribute.CanonicalTitle, attribute.Titles.JaJp, attribute.Titles.En, attribute.Titles.EnUs,],
            TitlePreferenceType.Canonical => [attribute.CanonicalTitle, attribute.Titles.EnJp, attribute.Titles.JaJp, attribute.Titles.En, attribute.Titles.EnUs,],
            _ => throw new ArgumentOutOfRangeException(nameof(preference), preference, null)
        };
}
