; (function ($) {
    "use strict";
    var $fades = $(".fadeThrough"),
        fadesCount = $fades.length,
        i = 0,
        fadeInterval = 4000,
        holdInterval = 15000,
        offsetInterval = holdInterval / fadesCount,
        nextOffset = offsetInterval,
        fadeItems = $.map($fades,function(el){ return { $items: $(el).children(), indx:0 }; }),
        changeList = function (fadeItems) {
            fadeItems.$items.eq(fadeItems.indx).fadeOut(fadeInterval, function () {
                fadeItems.indx += 1;
                if (fadeItems.indx === fadeItems.$items.length) {
                    fadeItems.indx = 0;
                }
                fadeItems.$items.eq(fadeItems.indx).fadeIn(fadeInterval);
                if (!fadeItems.intervalSet) {
                    fadeItems.intervalSet = true;
                    window.setTimeout(function () { changeList(fadeItems); }, holdInterval);
                }
            });
        };
    $.each(fadeItems, function (indx,el) {
        el.$items.hide().first().show();
        window.setTimeout(function () { changeList(el); }, nextOffset);
        nextOffset += offsetInterval;
    });
})(jQuery);

; (function ($) {
    "use strict";
    var detailLink = $("<a>see more…</a>"),
        $bioDlg = $("#bioModal"),
        wordCount = 14,
        selectFirst = new RegExp("(\\S+\\s+){" + wordCount + "}"),
        nonWhiteSpace = /\S/,
        shorten1stTextNode = function (el) {
            var children = el.childNodes, child, i=0;
            for (; i < children.length; i++) {
                child = children[i];
                if (child.tagName == "P") {
                    var grandChildren = child.childNodes, grandChild, j = 0;
                    for (; j < grandChildren.length; j++) {
                        grandChild = grandChildren[j];
                        if (grandChild.nodeType === 3 && nonWhiteSpace.test(grandChild.textContent)) {
                            var match = selectFirst.exec(grandChild.textContent);
                            if (match) {
                                grandChild.textContent = match[0];
                            }
                            i += 1;
                            while (el.childNodes.length > i) {
                                el.removeChild(el.lastChild);
                            }
                            return;
                        }
                    }
                }
            }
        };
    $("dd").each(function (index, el) {
        var $el = $(el),
            inner = el.innerHTML,
            title = $el.prev().text(),
            className = /panel-\w+/.exec($el.closest('.panel').attr('class'))[0];
        shorten1stTextNode(el);
        detailLink.clone().appendTo($el.children('p:first')).on("click", function () {
            var $bioPhoto;
            $(".modal-body", $bioDlg)[0].innerHTML = inner;
            $(".modal-title", $bioDlg).text(title);
            $(".modal-content", $bioDlg).attr('class', 'modal-content ' + className)
            $bioPhoto = $(".bioPhoto", $bioDlg);
            $bioPhoto.attr('class', $bioPhoto.attr('class').replace(/(col-\w\w)-\d+/g, "$1-4"));
            $bioDlg.modal('show');
        });
    });

})(jQuery);