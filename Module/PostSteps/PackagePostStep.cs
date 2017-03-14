using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Indexing;
using Sitecore.ContentSearch.Maintenance;

namespace RecommendedRenderings.PostSteps
{
    public class PackagePostStep : Sitecore.Install.Framework.IPostStep
    {
        public void Run(Sitecore.Install.Framework.ITaskOutput output, System.Collections.Specialized.NameValueCollection metaData)
        {
            try
            {
                var meta = metaData["Attributes"]; 
                Sitecore.Diagnostics.Log.Info("Recommended Renderings Metadata:" + meta, this);

                var attribute = meta.Split('|')[0];
                var attributeValue = attribute.Split('=')[1];

                foreach (var s in attributeValue.Split(';'))
                {
                    Sitecore.Diagnostics.Log.Info("Recommended Renderings Index: " + s, this);

                    Sitecore.ContentSearch.ISearchIndex index = Sitecore.ContentSearch.ContentSearchManager.GetIndex(s);
                    if (index != null)
                    {
                        index.Rebuild(Sitecore.ContentSearch.IndexingOptions.ForcedIndexing);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Recommended Renderings Exception: " + ex, this);
            }
        }
    }
}