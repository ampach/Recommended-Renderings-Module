using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using RecommendedRenderings.Models;
using Sitecore.Data;

namespace RecommendedRenderings.Services
{
    public class RecommendedRenderingsService
    {
        private const string sourceCacheKey = "GetInitialSource_";

        public TreeviewRoot GetInitialSource()
        {

            if (ID.IsNullOrEmpty(RecommendedRenderingsContext.CurrentItemID))
            {
                return new TreeviewRoot(); 
            }

            if (HttpContext.Current.Cache[sourceCacheKey + RecommendedRenderingsContext.CurrentItemID.ToShortID()] != null)
            {
                return (TreeviewRoot)HttpContext.Current.Cache[sourceCacheKey + RecommendedRenderingsContext.CurrentItemID.ToShortID()];
            }

            var root = new TreeviewRoot();
            var searchResult = SearchService.Search(RecommendedRenderingsContext.CurrentItem, RecommendedRenderingsContext.ContentRootItem, RecommendedRenderingsContext.PlaceholderSettingsRootItem);
            
            if (searchResult == null) return root;

            root.Items.AddRange(searchResult.Renderings.Select(q => RecommendedRenderingsContext.Database.GetItem(ShortID.Parse(q.RenderingID).ToID())).Where(q => q != null).ToList());

            HttpContext.Current.Cache.Insert(sourceCacheKey + RecommendedRenderingsContext.CurrentItemID.ToShortID(), root, null, DateTime.Now.AddMinutes(1), Cache.NoSlidingExpiration);

            return root;
        }

    }


}