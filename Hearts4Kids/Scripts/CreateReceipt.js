; (function ($) {
    $('.datepicker').datepicker("option", "maxDate", new Date());

    $('#IsOrganisation').on('change', function () {
        $('#WebUrl,#logoImg,#LogoSrc').prop('disabled', !this.checked).toggleClass('disabled', !this.checked);
    }).trigger('change');

    $('#TransferMethodId').on('change', function () {
        var selval = $(this).val();
        $('#Description').prop('disabled', selval != 3 && selval != 4);
    }).trigger('change');

    var mainForm = $('#createReceiptForm');
    function updateOnAjax(data) {
        if (!data) { return; }
        var p, v, $els = $('input,select,textarea', mainForm);
        for (p in data) {
            if (data.hasOwnProperty(p)) {
                v = data[p];
                if (p == "ExistingReceipts") {
                    $('#priorInfo').css('visibility', v.length > 0 ? 'visible' : 'hidden');
                    $('#priorInfoData').html('<tr>' + $.map(v, function (el, indx) {
                        return '<td>' + el.DateString + '</td>' + '<td>$' + el.Amount.toFixed(2) + '</td>';
                    }).join('</tr><tr>') + '</tr>');
                }
                else if (typeof v == "boolean") {
                    $els.filter('[name="' + p + '"]').prop('checked', v).trigger('change');
                } else {
                    $els.filter('[name="' + p + '"]').val(v).trigger('change');
                }
            }
        }
    }
    var lastEmail;
    function emailSelected() {
        var val = (arguments.length >= 2 && arguments[1].item)
            ? arguments[1].item.value
            : this.value;
        if (!val || lastEmail == val) { return; }
        lastEmail = val;
        $.ajax({
            url: "GetDonorInfo/",
            data: { email: val },
            dataType: "json",
            type: "POST"
        }).success(updateOnAjax);
    }

    $('#Email', mainForm).autocomplete(
        {
            //delay: 100,
            minLength: 1,
            source: function (request, response) {
                $.ajax({
                    url: "GetEmails/",
                    data: { startsWith: request.term },
                    dataType: "json",
                    type: "POST"
                }).success(response);
            },
            select: emailSelected
        })
        .on('blur', emailSelected);


})(jQuery);
