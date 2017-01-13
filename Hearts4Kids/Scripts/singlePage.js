//identify all containers with .fadeThrough, and fade the children 1 at a time but each container out of phase
(function ($) {
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
    $(window).ready(function(){
        $.each(fadeItems, function (indx, el) {
            var maxHeight = 0,
                $parent,
                imgLoaded = function () {
                    var imgHt = $(this).height();
                    if (imgHt > maxHeight) {
                        maxHeight = imgHt;
                        $parent.css('height', maxHeight);
                    }
                };
            el.$items.each(function (indx, el) {
                var $t = $(el),
                    ht = $t.outerHeight();
                $('img', el).on('load', imgLoaded);
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
    if (!$banner.length) { return; }
    parseCss('LandingPage', function (cssStr) {
        var move = /100%\s*{\s*transform:\s*translateX\(([-\d]+)px\)/.exec(cssStr)[1] + "px",
            rpt = /\banimation-duration:\s*(\d+)\s*s/.exec(cssStr)[1] * 1000;
        anim = function () {
            $banner.css('left', 0)
                .animate({
                    left: move
                }, rpt, 'linear', function () {
                    anim();
                });
        };
        anim();
    });
    
})(jQuery);

//links for the bios page - truncate to a few words and smaller photo - show details in bootstrap dialog
(function ($) {
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
                if (child.tagName === "P") {
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
            $(".modal-content", $bioDlg).attr('class', 'modal-content ' + className);
            $bioPhoto = $(".bioPhoto", $bioDlg);
            $bioPhoto.attr('class', $bioPhoto.attr('class').replace(/(col-\w\w)-\d+/g, "$1-4"));
            $bioDlg.modal('show');
            if (Modernizr.history) {
                var addrBar = location.href.split("?"),
                    name = $(this).closest("dd").prev().text().split("(")[0].trim();
                if (addrBar[1]) {//because dialog is modal and removes querystring on close (via back), must be direct referal here
                    history.replaceState(null,name,addrBar[0]);
                } 
                history.pushState(null, name, addrBar[0] + "?name=" + encodeURIComponent(name));
            }
        });
    }).length){
        var name = getParameterByName('name');
        if (Modernizr.history) {
            $bioDlg.on('hidden.bs.modal', function () {
                history.back();
            });
        }
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
        e.preventDefault();

        $.ajax({ url: e.target.action, method: 'post', data: $(e.target).serialize() });
    });
    
})(jQuery);

(function ($) {
    var token;
    $.ajaxSetup({
        data: { __RequestVerificationToken: token || (token = $('input[name="__RequestVerificationToken"]').val()) }
    });
})(jQuery);

(function ($) {
    var $sel = $('select');
    if (!$sel.length) {return;}
    var rgx = /([a-z0-9])([A-Z])/g,
        hasSpace = function(option,indx){
            return $(option).text().indexOf(" ") > -1;
        };
    // Production steps of ECMA-262, Edition 5, 15.4.4.17
    // Reference: http://es5.github.io/#x15.4.4.17
    if (!Array.prototype.some) {
        Array.prototype.some = function(fun/*, thisArg*/) {
            'use strict';

            if (this === null) {
                throw new TypeError('Array.prototype.some called on null or undefined');
            }

            if (typeof fun !== 'function') {
                throw new TypeError();
            }

            var t = Object(this);
            var len = t.length >>> 0;

            var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
            for (var i = 0; i < len; i++) {
                if (i in t && fun.call(thisArg, t[i], i, t)) {
                    return true;
                }
            }

            return false;
        };
    }
    
    $sel.each(function () {
        var $options = $('option',this).not(':first');
        if (!Array.prototype.some.call($options,hasSpace)){
            $options.each(function (indx, el) {
                var $el = $(el);
                $el.text($el.text().replace(rgx, '$1 $2'));
            });
        }
    });

})(jQuery);


(function ($) {
    $('.btn-file>input[type="file"]').on('change', function () {
        $('.urlField',$(this).closest('.input-group'))[0].value = this.value.replace(/\\/g, '/').replace(/.*\//, '');
    });
})(jQuery);



function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function parseCss(href, useStringFn) {
    var sheets = $('link[rel="stylesheet"][href*="' + href + '"]');
    $.ajax({
        url: sheets[sheets.length - 1].href,
        dataType: "text"
    }).success(function (cssText) {
        useStringFn(cssText);
    });
}