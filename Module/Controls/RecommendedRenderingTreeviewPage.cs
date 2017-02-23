using System;
using System.Web.UI;
using RecommendedRenderings.Services;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace RecommendedRenderings.Controls
{
    public class RecommendedRenderingTreeviewPage : Page
    {
        private readonly RecommendedRenderingsService _service = new RecommendedRenderingsService();

        /// <summary>Handles the Load event of the Page control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object)e, "e");
            RecommendedRenderingTreeview treeviewExtended = new RecommendedRenderingTreeview();
            this.Controls.Add((Control)treeviewExtended);
            treeviewExtended.ID = WebUtil.GetQueryString("treeid");

            if (!Sitecore.Data.ID.IsNullOrEmpty(RecommendedRenderingsContext.CurrentItemID))
            {
                treeviewExtended.DataSource = _service.GetInitialSource();
            }
            
            treeviewExtended.SelectedItem = "0";
        }
    }
}
