if (typeof(Sitecore) == "undefined") {
  Sitecore = new Object();
}

Sitecore.RecommendedRenderingTreeview = new function () {
};


Sitecore.RecommendedRenderingTreeview.onTreeClick = function (element, evt, click) {
  var source = Event.element(evt);
  var node = source.up("div.scContentTreeNode");
  if (node == null || node.id == null || node.id == "") {
    return;
  }

  var id = node.id.substr(node.id.lastIndexOf("_") + 1)

  if (source.className == "scContentTreeNodeGlyph") {
      return Sitecore.RecommendedRenderingTreeview.onTreeGlyphClick(node, $(element), id);
  }

  return Sitecore.Treeview.onTreeNodeClick(node, $(element), evt, id, click);
};

Sitecore.RecommendedRenderingTreeview.onTreeGlyphClick = function (node, treeElement, id) {
  var glyph = node.down();

  if (glyph.src.indexOf("treemenu_collapsed.png") >= 0 && glyph.src.indexOf("notreemenu_collapsed.png.gif") == -1) {
      Sitecore.Treeview.setGlyph(glyph, "/sc-spinner16.gif");

    var content = $F(treeElement.id + "_Database");

    body = treeElement.id + "_Selected=" + escape($F(treeElement.id + "_Selected")) + "&" + treeElement.id + "_Parameters=" + escape($F(treeElement.id + "_Parameters"));
    var templateIDs = $(treeElement.id + "_templateIDs");
    if (templateIDs) {
      body += "&" + treeElement.id + "_templateIDs=" + escape(templateIDs.value);
    }
    var displayFieldName = $(treeElement.id + "_displayFieldName");
    if (displayFieldName) {
      body += "&" + treeElement.id + "_displayFieldName=" + escape(displayFieldName.value);
    }

      var itemId;
    var itemIdField = window.document.getElementById(treeElement.id + "_ItemID");
    if (itemIdField) {
        itemId = itemIdField.value;
    } else {
        itemId = "";
    }
 
    if (window.scCSRFToken && window.scCSRFToken.key && window.scCSRFToken.value) {
        body += "&" + window.scCSRFToken.key + "=" + window.scCSRFToken.value;
    }

    var contentLanguage;
    var treeviewLanguage = window.document.getElementById(treeElement.id + "_Language");

    if (treeviewLanguage) {
      contentLanguage = "&la=" + treeviewLanguage.value;
    } else {
      contentLanguage = "";
    }

    new Ajax.Request("/sitecore/shell/Controls/RecommendedRenderingTreeview/RecommendedRenderingTreeview.aspx?treeid=" + encodeURIComponent(treeElement.id) + "&id=" + encodeURIComponent(id) + "&ciId=" + itemId + (content != null ? "&sc_content=" + content : "") + contentLanguage, {
      method: "post",
      postBody: body,
        onSuccess: function (transport) { Sitecore.Treeview.expandTreeNode(node, transport.responseText) },
        onException: function (request, ex) { alert(ex); },
        onFailure: function (request) { alert("Failed"); }
    });
  } 
  else if (glyph.src.indexOf("treemenu_expanded.png") > 0) {
      Sitecore.Treeview.collapseTreeNode(node);
  }

  return false;
};