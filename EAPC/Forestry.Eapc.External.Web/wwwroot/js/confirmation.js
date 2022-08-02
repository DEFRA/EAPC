$(function () {
    $(document).ready(setButtonState);

    $('#Confirmation_AcceptTermsAndConditions').on('change', setButtonState);

    function setButtonState() {
        const isDraft = $('#application-state').text().toLowerCase() === 'draft';
        const isAccepted = $('#Confirmation_AcceptTermsAndConditions').prop("checked") === true;

        if (isAccepted && isDraft) {
            $('#submit-application').removeAttr('disabled');
        } else {
            $('#submit-application').attr('disabled', 'disabled');
        }
    }
});