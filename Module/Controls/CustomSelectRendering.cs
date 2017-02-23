using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using RecommendedRenderings.Services;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Version = Sitecore.Data.Version;


namespace RecommendedRenderings.Controls
{
    public class CustomSelectRendering : Sitecore.Shell.Applications.Dialogs.SelectRendering.SelectRenderingForm
    {
        private readonly RecommendedRenderingsService _service = new RecommendedRenderingsService();
        protected RecommendedRenderingTreeview RecommendedTreeview;
        protected Scrollbox RecommendedPlaceholders;

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");

            RenderPlaceholderRecomendations();

            if (!ID.IsNullOrEmpty(RecommendedRenderingsContext.CurrentItemID) && RecommendedTreeview != null)
            {
                RecommendedTreeview.DataSource = _service.GetInitialSource();
            }
            base.OnLoad(e);
        }

        private void RenderPlaceholderRecomendations()
        {
            var listOfSelectedRenderings = new List<Item>();

            if (HttpContext.Current.Request.Form["__EVENTTARGET"] != null)
            {
                if(Treeview != null && HttpContext.Current.Request.Form["__EVENTTARGET"] == Treeview.ID) { 
                    if (Treeview.SelectedIDs.Any(q => !String.IsNullOrWhiteSpace(q)))
                    {
                        var selected = Treeview.GetSelectedItems();
                        if (selected != null)
                        {
                            var selectedrendering = selected.FirstOrDefault();
                            if (selectedrendering != null)
                            {
                                listOfSelectedRenderings.Add(selectedrendering);
                            }
                        }
                    }
                }

                if (RecommendedTreeview != null && HttpContext.Current.Request.Form["__EVENTTARGET"] == RecommendedTreeview.ID)
                {
                    var rrSelected = RecommendedTreeview.GetSelectionItem();
                    if (rrSelected != null)
                    {
                        var selectedrendering = rrSelected.FirstOrDefault();
                        if (selectedrendering != null)
                        {
                            listOfSelectedRenderings.Add(selectedrendering);
                        }
                    }
                }


                if (listOfSelectedRenderings.Any())
                    RenderPlaceholderRecomendations(listOfSelectedRenderings.First());
                else
                    RecommendedPlaceholders.InnerHtml = String.Empty;
            }

            
        }

        private void RenderPlaceholderRecomendations(Item item)
        {
            if (!IsItemRendering(item))
            {
                RecommendedPlaceholders.InnerHtml = String.Empty;
                return;
            }

            var recommendedPlaceholders = Services.SearchService.Search(item.ID.ToShortID().ToString());
            if (recommendedPlaceholders != null && recommendedPlaceholders.Any())
            {
                HtmlTextWriter output = new HtmlTextWriter((TextWriter)new StringWriter());

                output.Write("<div class='rpContainer' style='display: none;'>");
                output.Write("<div class='close-button' onclick=\"javascript:return Sitecore.RecommendedPlaceholders.clocetPlaceholdersPopup()\">");
                output.Write("Choose one of recommended placeholders:");
                output.Write("</div>");

                foreach (var placeholder in recommendedPlaceholders)
                {
                    output.Write("<div class='rplItem' data-value=\"" + placeholder + "\" onclick=\"javascript:return Sitecore.RecommendedPlaceholders.setPlaceholder(this)\">");
                    output.Write(placeholder);
                    output.Write("</div>");
                }
                output.Write("</div>");

                RecommendedPlaceholders.InnerHtml = output.InnerWriter.ToString();
            }
        }

        [UsedImplicitly]
        protected void RecommendedrenderingTreeview_Click()
        {
            var selectionItems = this.RecommendedTreeview.GetSelectionItem();

            if (selectionItems != null) 
            {
                this.SelectedItemId = string.Empty;
                this.Renderings.InnerHtml = selectionItems.Count == 1 ? RenderEmptyPreview(selectionItems.First()) : RenderPreviews(selectionItems);
            }
            SetOpenPropertiesState(selectionItems != null ? selectionItems.FirstOrDefault() : null);
        }

        private string RenderEmptyPreview(Item item)
        {
            HtmlTextWriter output = new HtmlTextWriter((TextWriter)new StringWriter());
            output.Write("<table class='scEmptyPreview'>");
            output.Write("<tbody>");
            output.Write("<tr>");
            output.Write("<td>");
            if (item == null)
                output.Write(Translate.Text("None available."));
            else if (this.IsItemRendering(item))
            {
                output.Write("<div class='scImageContainer'>");
                output.Write("<span style='height:100%; width:1px; display:inline-block;'></span>");
                string str = item.Appearance.Icon;
                int num1 = 48;
                int num2 = 48;
                if (!string.IsNullOrEmpty(item.Appearance.Thumbnail) && item.Appearance.Thumbnail != Sitecore.Configuration.Settings.DefaultThumbnail)
                {
                    string thumbnailSrc = UIUtil.GetThumbnailSrc(item, 128, 128);
                    if (!string.IsNullOrEmpty(thumbnailSrc))
                    {
                        str = thumbnailSrc;
                        num1 = 128;
                        num2 = 128;
                    }
                }
                new ImageBuilder()
                {
                    Align = "absmiddle",
                    Src = str,
                    Width = num2,
                    Height = num1
                }.Render(output);
                output.Write("</div>");
                output.Write("<span class='scDisplayName'>");
                output.Write(item.DisplayName);
                output.Write("</span>");
                
            }
            else
                output.Write(Translate.Text("Please select a rendering item"));
            output.Write("</td>");
            output.Write("</tr>");
            output.Write("</tbody>");
            output.Write("</table>");
            return output.InnerWriter.ToString();
        }

        private string RenderPreviews(IEnumerable<Item> items)
        {
            Assert.ArgumentNotNull((object)items, "items");
            HtmlTextWriter output = new HtmlTextWriter((TextWriter)new StringWriter());
            bool flag = false;
            foreach (Item obj in items)
            {
                this.RenderItemPreview(obj, output);
                flag = true;
            }
            if (!flag)
                return this.RenderEmptyPreview((Item)null);
            return output.InnerWriter.ToString();
        }

        private void SetOpenPropertiesState(Item item)
        {
            if (item == null || !this.IsItemRendering(item))
            {
                this.OpenProperties.Disabled = true;
                this.OpenProperties.Checked = false;
            }
            else
            {
                switch (item["Open Properties After Add"])
                {
                    case "-":
                    case "":
                        this.OpenProperties.Disabled = false;
                        this.OpenProperties.Checked = this.IsOpenPropertiesChecked;
                        break;
                    case "0":
                        if (!this.OpenProperties.Disabled)
                            this.IsOpenPropertiesChecked = this.OpenProperties.Checked;
                        this.OpenProperties.Disabled = true;
                        this.OpenProperties.Checked = false;
                        break;
                    case "1":
                        if (!this.OpenProperties.Disabled)
                            this.IsOpenPropertiesChecked = this.OpenProperties.Checked;
                        this.OpenProperties.Disabled = true;
                        this.OpenProperties.Checked = true;
                        break;
                }
            }
        }

        /// <summary>Handles click on selectable item</summary>
        /// <param name="id">The ID of the item</param>
        /// <param name="language">The language</param>
        /// <param name="version">The version</param>
        [UsedImplicitly]
        protected override void SelectableItemPreview_Click(string id, string language, string version)
        {
            Assert.ArgumentNotNull((object)id, "id");
            Assert.ArgumentNotNull((object)language, "language");
            Assert.ArgumentNotNull((object)version, "version");

            Item obj = RecommendedRenderingsContext.Database.GetItem(id, Language.Parse(language), Version.Parse(version));
            if (obj != null)
            {
                RenderPlaceholderRecomendations(obj);
            }

            base.SelectableItemPreview_Click(id, language, version);
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object)args, "args");
            this.OK_Click();
        }

        protected new void OK_Click()
        {
            Item selectionItem = this.Treeview.GetSelectionItem();
            Item RecommendedItem = this.RecommendedTreeview.GetSelectionItem().FirstOrDefault();
            if (RecommendedItem == null && selectionItem == null)
            {
                SheerResponse.Alert("Select an item.");
            }
            else
            {
                Item resultItem = RecommendedItem != null ? RecommendedItem : selectionItem;

                bool flag = true;
                if (!string.IsNullOrEmpty(this.IncludeTemplateForSelection))
                    flag = this.IncludeTemplateForSelection.Contains("," + (object)resultItem.TemplateID + ",");
                else if (!string.IsNullOrEmpty(this.ExcludeTemplateForSelection))
                    flag = !this.ExcludeTemplateForSelection.Contains("," + (object)resultItem.TemplateID + ",");
                if (!flag)
                {
                    SheerResponse.Alert("Select an item.");
                }
                else
                {
                    this.SetDialogResult(resultItem);
                    SheerResponse.CloseWindow();
                }
            }
        }
    }
}