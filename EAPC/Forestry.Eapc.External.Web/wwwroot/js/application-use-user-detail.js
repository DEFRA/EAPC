$(function() {
    function applyValue(name) {
        const hiddenText = $.find('input[type="hidden"][name="' + name + '"]');
        const target = $.find('input[data-user-profile-source="' + name + '"]');
        if (hiddenText.length === 0 || target.length === 0) {
            return;
        }

        $(target).val($(hiddenText).val());
    }


    const buttons = $.find('button[data-purpose="PopulateUserDetail"]');

    for (let i = 0; i < buttons.length; i++) {
        const button = $(buttons[i]);
        button.on('click', function() {
            applyValue('FullName');
            applyValue('StreetAddressLine1');
            applyValue('StreetAddressLine2');
            applyValue('StreetAddressLine3');
            applyValue('StreetAddressLine4');
            applyValue('PostalCode');
        });
    }
});