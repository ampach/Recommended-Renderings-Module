if (typeof (Sitecore) == "undefined") {
    Sitecore = new Object();
}

Sitecore.RecommendedPlaceholders = new function () {
};

Sitecore.RecommendedPlaceholders.setPlaceholder = function (el) {
    var selectedValue = el.getAttribute("data-value");
    if (selectedValue.length) {
        document.getElementById("PlaceholderName").value = selectedValue;
        var item = document.getElementsByClassName("Recommended-placeholders")[0];
        item.style.display = 'none';
    }
};

Sitecore.RecommendedPlaceholders.clocetPlaceholdersPopup = function () {
    var item = document.getElementsByClassName("Recommended-placeholders")[0];
    item.style.display = 'none';
    e.preventDefault();
};



document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("PlaceholderName").addEventListener("click", function (e) {
        if (document.getElementsByClassName("Recommended-placeholders")[0].children.length > 0) {
            toggleItem("Recommended-placeholders");
        }
        e.preventDefault();
    });

    document.addEventListener("click", function (e) {
        if (closestByClass(e.target, "extended-placeholder-container") == null) {
            var item = document.getElementsByClassName("Recommended-placeholders")[0];
            item.style.display = 'none';
        }
        e.preventDefault();
    });

    
});

var closestByClass = function (el, classname) {
    while (el.className != classname) {
        el = el.parentNode;
        if (!el) {
            return null;
        }
    }
    return el;
}

var toggleItem = function(classname){
    var item = document.getElementsByClassName(classname)[0];
    if (item.style.display == "block")
    {
        item.style.display = 'none';
    }
    else
    {
        item.style.display = 'block';
    }
}