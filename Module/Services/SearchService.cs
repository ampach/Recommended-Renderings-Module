using System;
using System.Collections.Generic;
using System.Linq;
using RecommendedRenderings.Models;
using RecommendedRenderings.Search;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace RecommendedRenderings.Services
{
    public static class SearchService
    {

        private static ISearchIndex _index;
        private static ISearchIndex _placeholdersIndex;

        private static ISearchIndex Index
        {
            get { return _index ?? (_index = ContentSearchManager.GetIndex(Configuration.Settings.SearchIndexName)); }
        }

        private static ISearchIndex PlaceholdersIndex
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Configuration.Settings.RecomendedRenderingsIndex))
                {
                    return Index;
                }
                return _placeholdersIndex ?? (_placeholdersIndex = ContentSearchManager.GetIndex(Configuration.Settings.RecomendedRenderingsIndex));
            }
        }

        public static RecommendedRenderingsSearchResult Search(Item item, Item contentFolder, Item placeholderSettingsFolder)
        {
            if (item == null || contentFolder == null || placeholderSettingsFolder == null) return null;

            using (var RecommendedRenderingSearchContext = Index.CreateSearchContext())
            {
                var RecommendedRenderingQueryable = RecommendedRenderingSearchContext.GetQueryable<RecommendedRenderingsSearchResultItem>();

                var globalFolter = PredicateBuilder.True<RecommendedRenderingsSearchResultItem>();

                globalFolter = globalFolter.And(itm => itm.Paths.Contains(contentFolder.ID));

                var queryBuilder = PredicateBuilder.False<RecommendedRenderingsSearchResultItem>();
                queryBuilder = queryBuilder.Or(itm => itm.ItemId == item.ID).Boost(5.0f);
                queryBuilder = queryBuilder.Or(itm => itm.TemplateId == item.TemplateID).Boost(4.0f);

                globalFolter = globalFolter.And(queryBuilder);

                RecommendedRenderingQueryable = RecommendedRenderingQueryable.Where(globalFolter);

                var searchresults = RecommendedRenderingQueryable.GetResults();

                if (searchresults.TotalSearchResults > 0)
                {
                    var renderingsCollection = new List<RecommendedRenderingModel>();
                    var placeholdersCollection = new List<string>();

                    foreach (var searchHit in searchresults.Hits)
                    {
                        var RecommendedRenderings = searchHit.Document.RecommendedRenderings;

                        if (RecommendedRenderings == null) continue;

                        foreach (var rendering in RecommendedRenderings)
                        {
                            ParseRecommendedRenderingField(rendering, renderingsCollection, placeholdersCollection);
                        }
                    }

                    var result = new RecommendedRenderingsSearchResult();
                    result.Placeholders = placeholdersCollection.Distinct().ToList();
                    result.Renderings = renderingsCollection;

                    using (var recommendedPlaceholdersSearchContext = PlaceholdersIndex.CreateSearchContext())
                    {
                        var RecommendedRenderingByPlaceholdersQueryable = recommendedPlaceholdersSearchContext.GetQueryable<RecommendedRenderingsSearchResultItem>();

                        globalFolter = PredicateBuilder.True<RecommendedRenderingsSearchResultItem>();

                        globalFolter = globalFolter.And(itm => itm.Paths.Contains(placeholderSettingsFolder.ID));

                        var placehodersFilter = PredicateBuilder.False<RecommendedRenderingsSearchResultItem>();
                        foreach (var placeholder in result.Placeholders)
                        {
                            placehodersFilter = placehodersFilter.Or(itm => itm.PlaceholderKey == placeholder);
                        }

                        globalFolter = globalFolter.And(placehodersFilter);

                        var searchPlaceholdersResults = RecommendedRenderingByPlaceholdersQueryable.Where(globalFolter).GetResults();

                        if (searchPlaceholdersResults.TotalSearchResults > 0)
                        {
                            foreach (var placeholder in searchPlaceholdersResults.Hits)
                            {
                                if (placeholder.Document.AllowedControls == null) continue;

                                foreach (var allowedControl in placeholder.Document.AllowedControls)
                                {
                                    result.Renderings.Add(new RecommendedRenderingModel
                                    {
                                        RenderingID = allowedControl.ToShortID().ToString(),
                                        Placeholder = placeholder.Document.PlaceholderKey
                                    });
                                }
                            }
                        }

                    }

                    result.Renderings = result.Renderings.GroupBy(q => q.RenderingID).Select(q => q.First()).ToList();

                    return result;
                }
                return null;
            }
        }
        public static List<string> Search(string renderingShortID)
        {
            ShortID renderingID;
            if (!ShortID.TryParse(renderingShortID, out renderingID))
            {
                return null;
            }

            using (IProviderSearchContext RecommendedRenderingSearchContext = Index.CreateSearchContext())
            {
                var RecommendedRenderingQueryable =
                    RecommendedRenderingSearchContext.GetQueryable<RecommendedRenderingsSearchResultItem>();

                var queryBuilder = PredicateBuilder.True<RecommendedRenderingsSearchResultItem>();

                queryBuilder = queryBuilder.And(itm => itm.RecommendedRenderings.Contains(renderingShortID));

                RecommendedRenderingQueryable = RecommendedRenderingQueryable.Where(queryBuilder);

                var searchresults = RecommendedRenderingQueryable.GetResults();

                var result = new List<string>();

                if (searchresults.TotalSearchResults > 0)
                {
                    
                    foreach (
                        var document in
                            searchresults.Hits.Select(q => q.Document)
                                .Where(q => q.RecommendedRenderings.Any(w => w.Contains(renderingShortID))))
                    {
                        foreach (var rendering in document.RecommendedRenderings.Where(w => w.Contains(renderingShortID))
                            )
                        {
                            result.Add(rendering.Split('|')[0]);
                        }
                    }
                    
                }

                using (var recommendedPlaceholdersSearchContext = PlaceholdersIndex.CreateSearchContext())
                {
                    var placeholderSettingsQueryable =
                    recommendedPlaceholdersSearchContext.GetQueryable<RecommendedRenderingsSearchResultItem>();

                    var placeholderSettingsQueryBuilder = PredicateBuilder.True<RecommendedRenderingsSearchResultItem>();

                    placeholderSettingsQueryBuilder = placeholderSettingsQueryBuilder.And(itm => itm.AllowedControls.Contains(renderingID.ToID()));

                    placeholderSettingsQueryable = placeholderSettingsQueryable.Where(placeholderSettingsQueryBuilder);

                    var placeholdersResult = placeholderSettingsQueryable.GetResults();

                    if (placeholdersResult.TotalSearchResults > 0)
                    {
                        foreach (var rendering in placeholdersResult.Hits)
                        {
                            if (String.IsNullOrWhiteSpace(rendering.Document.PlaceholderKey)) continue;

                            result.Add(rendering.Document.PlaceholderKey);
                        }
                    }
                }

                if (result.Any())
                {
                    return result.Distinct().ToList();
                }

                return null;
            }
        }

        private static void ParseRecommendedRenderingField(string value, List<RecommendedRenderingModel> renderingsCollection, List<string> placeholdersCollection)
        {
            if (String.IsNullOrWhiteSpace(value))
                return;

            var firstSplit = value.Split('|');

            placeholdersCollection.Add(firstSplit[0]);

            var listOfIDs = firstSplit[1].Split(';');

            renderingsCollection.AddRange(listOfIDs.Select(r => new RecommendedRenderingModel
            {
                RenderingID = r,
                Placeholder = firstSplit[0]
            }));
        }
    }
}