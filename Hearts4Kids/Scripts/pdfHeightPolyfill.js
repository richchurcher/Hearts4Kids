;Modernizr.addTest('cssvhunit', function () {
    var bool;
    Modernizr.testStyles("#modernizr { height: 50vh; }", function (elem, rule) {
        var height = parseInt(window.innerHeight / 2, 10),
            compStyle = parseInt((window.getComputedStyle ?
                      getComputedStyle(elem, null) :
                      elem.currentStyle)["height"], 10);

        bool = (compStyle == height);
    });
    return bool;
});

(function () {
    if (!Modernizr.cssvhunit) {
        var winHeight;
        parseCss("DisplayPdf", function (cssText) {
            var vh = /height:\s*(\d+)vh/.exec(cssText);

            $('#pdf').height(winHeight * vh[1]/100);
        });
        winHeight = window.innerHeight || document.documentElement.offsetHeight;
    }
})();