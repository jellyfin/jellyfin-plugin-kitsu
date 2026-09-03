using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Kitsu.Configuration
{
    public enum TitlePreferenceType
    {
        /// <summary>
        /// Use titles in English.
        /// </summary>
        English,

        /// <summary>
        /// Use titles in Japanese.
        /// </summary>
        Japanese,

        /// <summary>
        /// Use titles in Japanese romaji.
        /// </summary>
        JapaneseRomaji,

        /// <summary>
        /// Use titles marked as canonical.
        /// </summary>
        Canonical
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        // store configurable settings your plugin might need
        public PluginConfiguration()
        {
            // set default options here
            TitlePreference = TitlePreferenceType.English;
            OriginalTitlePreference =  TitlePreferenceType.Canonical;
        }

        public TitlePreferenceType TitlePreference { get; set; }
        public TitlePreferenceType OriginalTitlePreference { get; set; }
    }
}
