using System.Collections.Generic;
using System.ComponentModel;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace RecommendedRenderings.Search
{
    public class RecommendedRenderingsSearchResultItem : SearchResultItem
    {
        [IndexField("recommended_rendering")]
        [TypeConverter(typeof(IndexFieldEnumerableConverter))]
        public IEnumerable<string> RecommendedRenderings { get; set; }

        [IndexField("allowed_controls")]
        [TypeConverter(typeof(IndexFieldEnumerableConverter))]
        public IEnumerable<ID> AllowedControls { get; set; }

        [IndexField("placeholder_key")]
        public virtual string PlaceholderKey { get; set; }

    }
}