using System;
using System.Web;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace RecommendedRenderings
{
    public class RecommendedRenderingsContext
    {
        private static readonly ItemsContext _items = new ItemsContext();

        public static ItemsContext Items
        {
            get
            {
                return _items;
            }
        }

        private const string ContextDataItemKey = "RR_CONTEXT_DATA";

        public static ContextData Data
        {
            get
            {
                ContextData contextData = HttpContext.Current.Items[ContextDataItemKey] as ContextData;
                if (contextData == null)
                {
                    contextData = new ContextData();
                    HttpContext.Current.Items[ContextDataItemKey] = (object)contextData;
                }
                return contextData;
            }
        }

        public static Database Database
        {
            get
            {
                var database = Data.Database;
                if (database != null)
                    return database;

                database = Factory.GetDatabase(Configuration.Settings.DatabaseName);
                if (database != null)
                {
                    Data.Database = database;
                    return Data.Database;
                }

                Data.Database = Sitecore.Context.ContentDatabase;

                return Data.Database;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                Data.Database = value;
            }
        }

        public static Item ContentRootItem
        {
            get
            {
                var contentRoot = Data.ContentRootItem;
                if (contentRoot != null)
                    return contentRoot;

                if (Database != null && !String.IsNullOrWhiteSpace(Configuration.Settings.ContentRootPath))
                {
                    Data.ContentRootItem = Database.GetItem(Configuration.Settings.ContentRootPath);
                    return Data.ContentRootItem;
                }

                return null;
            }
        }

        public static Item PlaceholderSettingsRootItem
        {
            get
            {
                var contentRoot = Data.PlaceholderSettingsRootItem;
                if (contentRoot != null)
                    return contentRoot;

                if (Database != null && !String.IsNullOrWhiteSpace(Configuration.Settings.PlaceholderSettingsRootPath))
                {
                    Data.PlaceholderSettingsRootItem = Database.GetItem(Configuration.Settings.PlaceholderSettingsRootPath);
                    return Data.PlaceholderSettingsRootItem;
                }

                return null;
            }
        }

        public static ID CurrentItemID
        {
            get
            {
                if (!ID.IsNullOrEmpty(Data.CurrentItemID))
                {
                    return Data.CurrentItemID;
                }

                var itemId = WebUtil.GetQueryString("ciId");
                if (!String.IsNullOrWhiteSpace(itemId))
                {
                    Data.CurrentItemID = ShortID.Parse(itemId).ToID();
                }
                else
                {
                    Data.CurrentItemID = ID.Null;
                }

                return Data.CurrentItemID;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                Data.CurrentItemID = value;
            }
        }

        public static Item CurrentItem
        {
            get
            {
                if (!ID.IsNullOrEmpty(CurrentItemID) && Database != null)
                {
                    Data.CurrentItem = Database.GetItem(CurrentItemID);
                    return Data.CurrentItem;
                }
                return null;
            }
        }

        public class ContextData
        {
            public Database Database { get; set; }
            public Item ContentRootItem { get; set; }
            public ID CurrentItemID { get; set; }
            public Item CurrentItem { get; set; }
            public Item PlaceholderSettingsRootItem { get; set; }
        }
    }
}