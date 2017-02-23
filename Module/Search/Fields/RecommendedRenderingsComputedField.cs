using System;
using System.Collections.Generic;
using System.Linq;
using RecommendedRenderings.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace RecommendedRenderings.Search.Fields
{
    public class RecommendedRenderingsComputedField : IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        /// <summary>
        /// Returns computed index field with list of Recommended Rendirings for Item per device
        /// </summary>
        /// <param name="indexable"></param>
        /// <returns></returns>
        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");

            try
            {
                var indexableItem = indexable as SitecoreIndexableItem;
                if (indexableItem == null)
                {
                    Log.Warn(string.Format("{0} : unsupported IIndexable type : {1}", this, indexable.GetType()), this);
                    return null;
                }

                var item = indexableItem.Item;

                if (!item.Paths.FullPath.StartsWith(Configuration.Settings.ContentRootPath)) return null;

                var result = new List<RecommendedRenderingModel>();

                ProcessLayoutDefinition(result, item);

                if (!result.Any())
                    return null;

                var totslResult = result.GroupBy(q => q.Placeholder).Select(q => new { Placeholder = q.Key, Renderings = q.GroupBy(w => w.RenderingID).Select(w => w.First())}).Select(q => q.Placeholder + "|" + q.Renderings.Select(r => r.RenderingID).Aggregate((f,s) => f + ";" + s));

                return totslResult;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        protected void ProcessLayoutDefinition(List<RecommendedRenderingModel> result, Item item)
        {
            using (new SecurityStateSwitcher(SecurityState.Disabled))
            {

#if (SC72 || SC75)
                Sitecore.Data.Fields.LayoutField layoutField = item.Fields["__Renderings"];
#else
                Sitecore.Data.Fields.LayoutField layoutField = item.Fields["__Final Renderings"];
#endif

                if (layoutField == null) return;

                // Get all devices
                DeviceRecords devices = item.Database.Resources.Devices;
                foreach (var deviceRecord in devices.GetAll())
                {
                    // Get the rendering references for the default device
                    Sitecore.Layouts.RenderingReference[] renderings = layoutField.GetReferences(deviceRecord);
                    if (renderings == null) continue;

                    foreach (var renderingReference in renderings)
                    {
                        result.Add(new RecommendedRenderingModel
                        {
                            Placeholder = renderingReference.Placeholder,
                            RenderingID = renderingReference.RenderingID.ToShortID().ToString()
                        });
                    }
                }
            }
        }

        //protected void ProcessPlaceholderSettings(List<RecommendedRenderingModel> result, List<string> placeholderKeys, Database contentDatabase)
        //{
        //    Assert.ArgumentNotNull((object)result, "result");
        //    Assert.ArgumentNotNull((object)placeholderKeys, "placeholderKeys");
        //    Assert.ArgumentNotNull((object)contentDatabase, "contentDatabase");

        //    foreach (var pl in placeholderKeys)
        //    {
        //        ProcessPlaceholderSettings(result, pl, contentDatabase);
        //    }
        //}

        //protected void ProcessPlaceholderSettings(List<RecommendedRenderingModel> result, string placeholderKey, Database contentDatabase)
        //{
            
        //    var placeholderSettings = contentDatabase.SelectItems(String.Format("fast:/sitecore/layout/Placeholder Settings//*[@Placeholder Key= '{0}']", placeholderKey));

        //    if (placeholderSettings == null)
        //        return;
            
        //    foreach (var ps in placeholderSettings)
        //    {
        //        var renderings = GetRenderings(ps);

        //        if (renderings == null) continue;

        //        result.AddRange(renderings.Select(q => new RecommendedRenderingModel
        //        {
        //            Placeholder = placeholderKey,
        //            RenderingID = ID.Parse(q).ToShortID().ToString(),
        //            Boost = 2
        //        }));
        //    }
        //}

        //protected void ProcessRelatedItems(List<RecommendedRenderingModel> result, Item item)
        //{
        //    var path = String.Format("fast:{0}//*[@@templatename = '{1}']", Configuration.Settings.ContentRootPath,
        //        item.TemplateName);
        //    var relatedItems = item.Database.SelectItems(path);

        //    foreach (var itrelatedItemem in relatedItems)
        //    {
        //        ProcessLayoutDefinition(result, itrelatedItemem);
        //    }

        //}

        //protected virtual List<string> GetRenderings(Item placeholderItem)
        //{
        //    Assert.ArgumentNotNull((object)placeholderItem, "placeholderItem");
        //    ListString listString = new ListString(placeholderItem["Allowed Controls"]);
        //    if (listString.Count <= 0)
        //        return null;
        //    return listString.ToList();
        //}
    }
}