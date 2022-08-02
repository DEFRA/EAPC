$(function () {
    const options = {
        "language": {
            "emptyTable": "No supporting documents have been uploaded.",
            "info": ""
        },
        "scrollY": 300,
        "scrollCollapse": true,
        "searching": false,
        "paging": false
    };

    $('#documentation-list-table').DataTable(options);
});

$(function () {
    bsCustomFileInput.init();

    $('#supporting-documentation-files').change(function (e) {

        $('#supporting-documentation-files-error').addClass('govuk-visually-hidden');
        $('#supporting-documentation-files-error').attr('aria-hidden', 'true');
        $('#submit-supporting-document').removeAttr('disabled');
        $('#supporting-documentation-files').removeClass('govuk-file-upload--error');
        $('#file-upload-error-text').html('');
        $('#file-select-group').removeClass('govuk-form-group--error');

        var fileExtensions = $('#allowed-extensions').val();
        var maxSize = $('#allowed-max-size').val();
        var maxSizeDescription = $('#allowed-max-size-description').val();
        var maxNumber = parseInt($('#allowed-number-documents').val());
        var currentNumber = parseInt($('#current-number-documents').val());

        var fileExtensionsArray = fileExtensions.split(', ');
        fileExtensionsArray[fileExtensionsArray.length - 1] = 'or ' + fileExtensionsArray[fileExtensionsArray.length - 1];
        var fileExtensionsDescription = fileExtensionsArray.join(', ');
        
        const files = e.target.files;
        const fileCount = files.length;
        const errors = [];

        if (fileCount === 0) {
            $('#supporting-documentation-files-label').text('Select a document');
            return;
        }

        if ((parseInt(fileCount) + currentNumber) > maxNumber) {
            errors.push('You can only upload up to ' + maxNumber + ' documents');
        }

        var stringLabelText = '';
        for (let i = 0; i < fileCount; i++) {

            if (files[i].size > maxSize) {
                errors.push(files[i].name + ' - files must be smaller than ' + maxSizeDescription);
            }

            var fileNameParts = files[i].name.split('.');
            var extension = fileNameParts[fileNameParts.length - 1].toLowerCase();
            if (fileExtensions.toLowerCase().includes(extension) === false) {
                errors.push(files[i].name + ' - selected files must be a ' + fileExtensionsDescription);
            }
            
            stringLabelText += '"' + files[i].name + '" ';
        }
        $('#supporting-documentation-files-label').text(stringLabelText.trim());

        if (errors.length > 0) {
            var errorText = errors.join('<br/>');
            $('#supporting-documentation-files-error').removeClass('govuk-visually-hidden');
            $('#supporting-documentation-files-error').attr('aria-hidden', 'false');
            $('#file-upload-error-text').html(errorText);
            $('#submit-supporting-document').attr('disabled', 'disabled');
            $('#supporting-documentation-files').addClass('govuk-file-upload--error');
            $('#file-select-group').addClass('govuk-form-group--error');
        }
    });
});

$(function () {
    const canBeEdited = $('#application-state').text().trim().toLowerCase() === 'submitted';
    if (canBeEdited) {
        $('.govuk-input, .govuk-select, .govuk-checkboxes__input, .govuk-textarea, .govuk-radios__item, .govuk-button--secondary, .govuk-file-upload, .govuk-button[name="store-supporting-documents"], .govuk-button[name="delete-supporting-document"], #supporting-documentation-files, [data-restrictions~="edit"]').each(function(){
            $(this).removeAttr('disabled');
        });
    }
});