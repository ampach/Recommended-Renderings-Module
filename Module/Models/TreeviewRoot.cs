using System.Collections.Generic;

namespace RecommendedRenderings.Models
{
    public class TreeviewRoot
    {
        public TreeviewRoot()
        {
            Items = new List<Sitecore.Data.Items.Item>();
            DisplayName = "Recommended Renderings";
            Value = Configuration.Settings.TreeviewRootDefaultValue;
            IsExpanded = Configuration.Settings.IsExpandedDefault;
            Icon = Configuration.Settings.TreeviewRootIcon;
        }

        public string DisplayName { get; set; }
        public string Value { get; set; }
        public string Icon { get; set; }
        public List<Sitecore.Data.Items.Item> Items { get; set; }
        public bool IsExpanded { get; set; }
    }
}