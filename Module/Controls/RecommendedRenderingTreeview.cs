using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using RecommendedRenderings.Models;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Text;
using Sitecore.Web;

namespace RecommendedRenderings.Controls
{
    public class RecommendedRenderingTreeview : System.Web.UI.WebControls.WebControl
    {
        public TreeviewRoot DataSource { get; set; }
        public string SelectedItem { get; set; }

        public string Click
        {
            get
            {
                return StringUtil.GetString(this.ViewState["Click"]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["Click"] = (object)value;
            }
        }
        
        protected virtual string GetHeaderValue(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            return string.IsNullOrEmpty(item.Appearance.DisplayName) ? item.Name : item.Appearance.DisplayName;
        }

        protected virtual string GetTreeNodeName(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            return Assert.ResultNotNull<string>(item.Appearance.DisplayName);
        }

        protected override void OnInit(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnInit(e);
            this.Page.ClientScript.RegisterClientScriptInclude("RecommendedRenderingTreeview", "/sitecore/shell/Controls/RecommendedRenderingTreeview/RecommendedRenderingTreeview.js");
        }
       
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
        }
        
        protected override void Render(HtmlTextWriter output) 
        {
            Assert.ArgumentNotNull((object)output, "output");

            if (DataSource == null || DataSource.Items == null || !DataSource.Items.Any())
            {
                this.Visible = false;
                return;
            }

            if (!String.IsNullOrEmpty(SelectedItem) && SelectedItem == "0")
            {
                foreach (var item in DataSource.Items)
                    this.RenderNode(output, item);
            }
            else
            {
                this.RenderTreeBegin(output);
                this.RenderTreeState(output);
                this.RenderRoot(output, DataSource);
                this.RenderTreeEnd(output);
            }
        }
        
        private static void RenderNodeEnd(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, "output");
            output.Write("</div>");
        }
        
        private static void RenderTreeNodeGlyph(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, "output");
            ImageBuilder imageBuilder = new ImageBuilder()
            {
                Class = "scContentTreeNodeGlyph"
            };
            imageBuilder.Src = "images/noexpand15x15.gif";
            output.Write((string) imageBuilder.ToString());
        }

        private static void RenderRootGlyph(HtmlTextWriter output, TreeviewRoot root, bool isExpanded)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)root, "root");
            ImageBuilder imageBuilder = new ImageBuilder()
            {
                Class = "scContentTreeNodeGlyph"
            };
            imageBuilder.Src = !isExpanded ? "images/treemenu_collapsed.png" : "images/treemenu_expanded.png";
            output.Write((string) imageBuilder.ToString());
        }

        private static void RenderRootGlyphSC7(HtmlTextWriter output, TreeviewRoot root, bool isExpanded)
        {
            Assert.ArgumentNotNull((object)output, "output");
            ImageBuilder imageBuilder = new ImageBuilder()
            {
                Class = "scContentTreeNodeGlyph"
            };
            imageBuilder.Src = !isExpanded ? "images/expand15x15.gif" : "images/collapse15x15.gif";
            output.Write(imageBuilder.ToString());
        }

        private static void RenderTreeNodeIcon(HtmlTextWriter output, Item item)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)item, "item");
            UrlBuilder urlBuilder = new UrlBuilder(item.Appearance.Icon);
            
            ImageBuilder imageBuilder = new ImageBuilder()
            {
                Src = urlBuilder.ToString(),
                Width = 16,
                Height = 16,
                Class = "scContentTreeNodeIcon"
            };
            if (!string.IsNullOrEmpty(item.Help.Text))
                imageBuilder.Alt = item.Help.Text;
            imageBuilder.Render(output);
        }

        private static void RenderRootIcon(HtmlTextWriter output, TreeviewRoot root)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)root, "item");
            
            ImageBuilder imageBuilder = new ImageBuilder()
            {
                Src = root.Icon,
                Width = 16,
                Height = 16,
                Class = "scContentTreeNodeIcon"
            };
            imageBuilder.Render(output);
        }

        protected virtual string GetNodeID(string shortID)
        {
            Assert.ArgumentNotNullOrEmpty(shortID, "shortID");
            return this.ID + "_" + shortID;
        }

        protected virtual void Render(HtmlTextWriter output, TreeviewRoot root)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)root, "root");

            RenderRoot(output, DataSource);
        }

        protected virtual void RenderTreeState(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, "output");
            output.Write("<input id=\"");
            output.Write(this.ID);
            output.Write("_Selected\" type=\"hidden\" value=\"\" />");
            output.Write("<input id=\"");
            output.Write(this.ID);
            output.Write("_Database\" type=\"hidden\" value=\"" + RecommendedRenderingsContext.Database.Name + "\" />");
            output.Write("<input id=\"");
            output.Write(this.ID);
            output.Write("_ItemID\" type=\"hidden\" value=\"" + RecommendedRenderingsContext.CurrentItemID.ToShortID() + "\" />");
            output.Write("<input id=\"");
            output.Write(this.ID);
            output.Write("_Parameters\" type=\"hidden\" value=\"\" />");
            
            
            output.Write("<input id=\"" + this.ID + "_Language\" type=\"hidden\" value=\"" + Sitecore.Context.Language.Name + "\"/>");
        }

        protected virtual void RenderTreeEnd(HtmlTextWriter output)
        {
            Assert.IsNotNull((object)output, "output");
            output.Write("</div>");
        }
   
        protected virtual void RenderTreeBegin(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, "output");
            output.Write("<div id=\"");
            output.Write(this.ID);
            output.Write("\" onclick=\"javascript:return Sitecore.RecommendedRenderingTreeview.onTreeClick(this,event");
            if (!string.IsNullOrEmpty(this.Click))
            {
                output.Write(",'");
                output.Write(StringUtil.EscapeQuote(this.Click));
                output.Write("'");
            }
            output.Write(")\"");
            output.Write(" onkeydown=\"javascript:return Sitecore.Treeview.onKeyDown(this,event)\"");
            if (this.Style.Count > 0)
                output.Write(" style='" + this.Style.Value + "'");
            output.Write(">");
        }

        private void RenderNode(HtmlTextWriter output, Item item)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)item, "item");
            this.RenderNodeBegin(output, item);
            RecommendedRenderingTreeview.RenderNodeEnd(output);
        }

        private void RenderRoot(HtmlTextWriter output, TreeviewRoot root)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)root, "root");
            
            this.RenderRootBegin(output, root, root.IsExpanded);
            if (root.IsExpanded)
            {
                if (root.Items != null)
                {
                    foreach (var item in root.Items)
                        this.RenderNode(output, item);
                }
            }
            RecommendedRenderingTreeview.RenderNodeEnd(output);
        }

        protected virtual void RenderNodeBegin(HtmlTextWriter output, Item item)
        {
            Assert.ArgumentNotNull((object)output, "output");
            Assert.ArgumentNotNull((object)item, "item");
            string currentId = item.ID.ToShortID().ToString();
            string nodeId = this.GetNodeID(currentId);
            output.Write("<div id=\"");
            output.Write(nodeId);
            output.Write("\" class=\"scContentTreeNode\">");
            RecommendedRenderingTreeview.RenderTreeNodeGlyph(output);
            string str = "scContentTreeNodeNormal";
            
            output.Write("<a href=\"#\" class=\"" + str + "\"");
            if (!string.IsNullOrEmpty(item.Help.Text))
            {
                output.Write(" title=\"");
                output.Write((string) StringUtil.EscapeQuote(item.Help.Text));
                output.Write("\"");
            }
            output.Write(">");
            RecommendedRenderingTreeview.RenderTreeNodeIcon(output, item);
            output.Write("<span hidefocus=\"true\" class=\"scContentTreeNodeTitle\" tabindex='0'>{0}</span>", (object)this.GetHeaderValue(item));
            output.Write("</a>");
        }

        protected virtual void RenderRootBegin(HtmlTextWriter output, TreeviewRoot root, bool isExpanded)
        {
            Assert.ArgumentNotNull((object) output, "output");
            Assert.ArgumentNotNull((object)root, "v");
            output.Write("<div id=\"");
            output.Write(root.Value);
            output.Write("\" class=\"scContentTreeNode\">");
#if (SC72 || SC75)
            RecommendedRenderingTreeview.RenderRootGlyphSC7(output, root, isExpanded);
#else
               RecommendedRenderingTreeview.RenderRootGlyph(output, root, isExpanded);
#endif


            string str = "scContentTreeNodeNormal";
            output.Write("<a href=\"#\" class=\"" + str + "\"");
            output.Write(">");
            RecommendedRenderingTreeview.RenderRootIcon(output, root);
            output.Write("<span hidefocus=\"true\" class=\"scContentTreeNodeTitle\" tabindex='0'>{0}</span>", (object)root.DisplayName);
            output.Write("</a>");
        }

        public List<Item> GetSelectionItem()
        {
            var lang = Language.Current;
            return GetSelectedItems(lang);
        }

        public List<Item> GetSelectedItems(Language language)
        {
            List<Item> objList = new List<Item>();
            var selectionItems = GetSelectedIDs();

            if (selectionItems == null)
            {
                return objList;
            }

            var isRootSelected = false;

            foreach (var selectedId in selectionItems)
            {
                if (!string.IsNullOrEmpty(selectedId) && selectedId == Configuration.Settings.TreeviewRootDefaultValue)
                {
                    isRootSelected = true;
                    break;
                }
                else
                {
                    string id = ShortID.Decode(selectedId);
                    Item obj = RecommendedRenderingsContext.Database.GetItem(id, language);
                    if (obj != null)
                        objList.Add(obj);
                }
            }

            if (isRootSelected)
            {
                if(DataSource != null && DataSource.Items != null)
                    return DataSource.Items;
            }

            return objList;
        }

        public List<string> GetSelectedIDs()
        {
            return new List<string>((IEnumerable<string>)WebUtil.GetFormValue(this.ID + "_Selected").Split(',').Where(q => q != this.ID && !string.IsNullOrWhiteSpace(q)));
        }
    }
}
