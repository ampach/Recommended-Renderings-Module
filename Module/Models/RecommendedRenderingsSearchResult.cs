using System.Collections.Generic;

namespace RecommendedRenderings.Models
{
    public class RecommendedRenderingsSearchResult
    {
        public List<string> Placeholders { get; set; }
        public List<RecommendedRenderingModel> Renderings { get; set; }

    }
}