$(function() {
    $('#Section5_Treatment').on('change', function (e) {
        const asJQuery = $(e.target);
        const siblingTextInput = asJQuery.siblings('input[type=text]');
        if (siblingTextInput.length === 0) {
            return;
        }

        $(siblingTextInput[0]).val('');

        const isOtherSelected = e.target.selectedIndex === (e.target.options.length - 1); // other is always last in the list
        if (isOtherSelected) {
            $(siblingTextInput).removeAttr('disabled');
        } else {
            $(siblingTextInput).attr('disabled', 'disabled');
        }
    });
});

$(function () {
    $('#Section5_Chemical').on('change', function (e) {
        const asJQuery = $(e.target);
        const siblingTextInput = asJQuery.siblings('input[type=text]');
        if (siblingTextInput.length === 0) {
            return;
        }

        $(siblingTextInput[0]).val('');

        const isOtherSelected = e.target.selectedIndex === (e.target.options.length - 1); // other is always last in the list
        if (isOtherSelected) {
            $(siblingTextInput).removeAttr('disabled');
        } else {
            $(siblingTextInput).attr('disabled', 'disabled');
        }
    });
});