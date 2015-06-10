$(function () {
    $('#fileupload').fileupload({
        dataType: "json",
        url: "/api/upload",
        limitConcurrentUploads: 1,
        sequentialUploads: true,
        progressInterval: 100,
        maxChunkSize: 10000,
        add: function (e, data) {
            $('#filelistholder').removeClass('hide');
            data.context = $('<div />').text(data.files[0].name).appendTo('#filelistholder');
            $('</div><div class="progress"><div class="bar" style="width:0%"></div></div>').appendTo(data.context);
            $('#btnUploadAll').click(function () {
                data.submit();
            });
        },
        done: function (e, data) {
            data.context.text(data.files[0].name + '... Completed');
            $('</div><div class="progress"><div class="bar" style="width:100%"></div></div>').appendTo(data.context);
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#overallbar').css('width', progress + '%');
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            data.context.find('.bar').css('width', progress + '%');
        }
    });
});