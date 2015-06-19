//identify all containers with .fadeThrough, and fade the children 1 at a time but each container out of phase
; (function ($) {
    "use strict";
    var $fades = $(".fadeThrough"),
        fadesCount = $fades.length,
        i = 0,
        fadeInterval = 4000,
        holdInterval = 15000,
        offsetInterval = holdInterval / fadesCount,
        fadeItems = $.map($fades,function(el){ return { $items: $(el).children(), indx:0 }; }),
        changeList = function () {
            var currentGroup;
            i += 1;
            if (i === fadeItems.length) { i = 0; }
            currentGroup = fadeItems[i];
            currentGroup.$items.eq(currentGroup.indx).fadeOut(fadeInterval, function () {
                currentGroup.indx += 1;
                if (currentGroup.indx === currentGroup.$items.length) {
                    currentGroup.indx = 0;
                }
                currentGroup.$items.eq(currentGroup.indx).fadeIn(fadeInterval);
            });
        };
    $(document).ready(function(){
        $.each(fadeItems, function (indx, el) {
            var maxHeight = 0,
                $parent;
            el.$items.each(function (indx, el) {
                var $t = $(el),
                    ht = $t.outerHeight();
                if (ht > maxHeight) {
                    maxHeight = ht;
                }
                if (indx > 0) {
                    $t.hide();
                } else {
                    $parent = $t.parent();
                }
            });
            if (maxHeight >= $parent.height()) {
                $parent.css('height', maxHeight);
            }
        });
    });
    if (fadesCount) { window.setInterval(changeList, offsetInterval); }
})(jQuery);

//manage width of photoBanner so it scrolls flawlessly
(function ($) {
    if (Modernizr.csstransforms) { return; }
    var $banner = $('.photobanner');
    if (!($banner || $banner.length)) { return; }
    var image_url = $banner.css('background-image'),
        image;

    // Remove url() or in case of Chrome url("")
    image_url = image_url.match(/^url\("?(.+?)"?\)$/);
    if (image_url[1]) {
        image_url = image_url[1];
        image = new Image();

        // just in case it is not already loaded
        $(image).load(function () {
            var width = image.width,
                containerWidth = $banner.parent().width(),
                nonRepeatWidth = width - 933,//1600 *7/12 - adjust if changed in photoServices const
                repeatAt = nonRepeatWidth + containerWidth + 'px',
                anim = function () {
                    $banner.css('left', 0)
                        .animate({
                            left: '-' + repeatAt
                        }, 120000, 'linear',function () {
                            anim();
                        });
                };
            anim();
        });
        image.src = image_url;
    }
})(jQuery);

//links for the bios page - truncate to a few words and smaller photo - show details in bootstrap dialog
; (function ($) {
    "use strict";
    var detailLink = $("<a class='seeMoreLink'>see more…</a>"),
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
    if ($("dd",$(".userBios")).each(function (index, el) {
        var $el = $(el),
            inner = el.innerHTML,
            title = $el.prev().text(),
            panelClass = $el.closest('.panel').attr('class'),
            className = panelClass? /panel-\w+/.exec(panelClass)[0] : 'panel-default';
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
    }).length){
        var name = getParameterByName('name');
        if (name) {
            var $foundItem;
            $("dt", $(".userBios")).each(function (indx, el) {
                var $t = $(el);
                if ($t.text().indexOf(name)>-1) {
                    $foundItem = $t;
                    return false;
                }
            });
            if ($foundItem) {
                $(".seeMoreLink", $foundItem.next()).trigger('click');
            }
        }
    }
})(jQuery);

(function ($) {
    $("form", $("#subscribeMenu").on("click", function (e) {
        if (e.target.type !== "submit") {
            e.stopPropagation();
        }
    })).on("submit", function (e) {
        var returnVar = {};
        e.preventDefault();
        $.each(e.target, function(indx, el){
            if (el.type !== "submit" && el.value) {
                returnVar[el.name || el.id] = el.value;
            }
        });
        $.ajax({ url: e.target.action, method: 'post', data: returnVar });
    });
    
})(jQuery);

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}