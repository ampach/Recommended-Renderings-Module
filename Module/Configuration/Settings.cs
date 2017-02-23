namespace RecommendedRenderings.Configuration
{
    public class Settings
    {
        public static string TreeviewRootDefaultValue
        {
            get
            {
                return "0";
            }
        }
        public static string SearchIndexName
        {
            get
            {
                return "recommended-renderings-index";
            }
        }
        public static string RecomendedRenderingsIndex
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("RecomendedRenderingsIndex");
            }
        }
        public static string TreeviewRootIcon
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("TreeviewRootIcon");
            }
        }

        public static string DatabaseName
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("DatabaseName");
            }
        }

        public static string ContentRootPath
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("ContentRootPath");
            }
        }

        public static string PlaceholderSettingsRootPath
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("PlaceholderSettingsRootPath");
            }
        }

        public static bool IsExpandedDefault
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting("IsExpandedDefault", false);
            }
        }
    }
}