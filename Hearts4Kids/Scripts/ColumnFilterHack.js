; (function ($) {
    var $thead = $('thead'),
        i=0,
        defer = function () {
            var fc = $('span.form-control', $thead);
            if (fc.length) {
                fc.removeClass('form-control');
            } else if (i < 50) {
                i++;
                setTimeout(defer, 20);
            }
        };
    defer();

})(jQuery);