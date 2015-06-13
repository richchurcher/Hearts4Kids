;(function ($) {
    $('.gallery').each(function (indx, el) {
        var links = el.getElementsByTagName('a');
        $(this).on("click", "a", function (a) {
            var target = a.target || a.srcElement,
			    link = target.src ? target.parentNode : target,
			    options = { index: link, event: a };
            blueimp.Gallery(links, options);
        });
    });
})(window.jQuery);