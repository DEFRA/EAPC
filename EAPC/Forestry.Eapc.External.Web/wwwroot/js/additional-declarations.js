(function ($) {
    const additionalDeclarationsNotRequiredId = 'Section6_AdditionalDeclarationsNotRequired';
    const additionalDeclarationsNotRequiredCheckBox = $('#' + additionalDeclarationsNotRequiredId);

    const modal = $('#additional-declarations-search');
    const errorDisplay = $('#additional-declarations-error');
    const loadingDisplay = $('#additional-declarations-loading');
    const noResultsDisplay = $('#additional-declarations-no-results');
    const help = $('#additional-declarations-search');
    const targetTextBox = $($.find('[data-role="additional-declarations-text"]'));
    const newLine = "\r\n";

    /**
     * Send the query to the server to retrieve recommended list of additional declarations.
     * @param {string} origin
     * @param {string} destination
     * @param {string} species
     * @param {string} treatment
     */
    function queryServer(origin, destination, species, treatment, onSuccess, onError) {
        const data = {
            CountryOfOrigin: origin,
            CountryOfDestination: destination,
            Species: species,
            Treatment: treatment
        };

        $.get('/api/AdditionalDeclarations', data)
            .done(onSuccess)
            .fail(onError);
    }

    function toggleAlertDisplayedToUser(value) {
        const allAlertDivs = [errorDisplay, loadingDisplay, noResultsDisplay];

        const hide = function (divs) {
            for (let i = 0; i < divs.length; i++) {
                divs[i].addClass('d-none');
                divs[i].attr('aria-hidden', true);
            }
        }

        const show = function (div) {
            div.removeClass('d-none');
            div.attr('aria-hidden', false);
        }

        if (!value) {
            // hide all
            hide(allAlertDivs);
            return;
        }

        for (let i = 0; i < allAlertDivs.length; i++) {
            const d = allAlertDivs[i];
            if (d === value) {
                show(d);
            } else {
                hide([d]);
            }
        }
    }

    $('#show-help').on('click', function() {
        help.toggle();
        $(this).text($(this).text() == 'Get suggestions' ? 'Hide suggestions' : 'Get suggestions');
    });

    $('#additional-declaration-trigger').on('click', function () {
        toggleAlertDisplayedToUser(loadingDisplay);

        const origin = modal.find('#additional-declarations-search-country-of-origin').val();
        const destination = modal.find('#additional-declarations-search-country-of-destination').val();
        const species = modal.find('#additional-declarations-search-species option:selected').val();
        const treatment = modal.find('#additional-declarations-search-treatment').val();

        const onSuccess = function(data) {
            const listGroup = $('#additional-declarations-list');
            listGroup.empty();

            if (data.length === 0) {
                toggleAlertDisplayedToUser(noResultsDisplay);
            } else {
                toggleAlertDisplayedToUser(); // no value provided which should hide all the alert divs
            }
            var declarationsContent='';
            for (let i = 0; i < data.length; i++) {
                if (i === 0) {
                    declarationsContent+='<div class="govuk-form-group">';
                    declarationsContent+='<div class="govuk-checkboxes govuk-checkboxes--small" data-module="govuk-checkboxes">';
                }
                
                const current = data[i].value;
                declarationsContent+='<div class="govuk-checkboxes__item">';
                declarationsContent+='<input type="checkbox" class="govuk-checkboxes__input" id="dec-' + i + '" value="'+current+'">';
                declarationsContent+='<label class="govuk-label govuk-checkboxes__label" for="dec-'+i+'">' + current + '</label>';
                declarationsContent+='</div>';

                if (i=== data.length-1)
                {
                    declarationsContent+='</div></div>';
                }
            }
            if (data.length) {
                listGroup.append('<h2 class="govuk-heading-s">Results</h2>');
                listGroup.append('<p class="govuk-body">Click each checkbox next to the declaration that you would like to add as an answer to the declaration question in this section. This will include it on your certificate application.</p>');
                listGroup.append(declarationsContent);
            }
        };
        const onError = function() {
            toggleAlertDisplayedToUser(errorDisplay);
        };

        queryServer(origin, destination, species, treatment, onSuccess, onError);
    });

    $(document).on('change', '[type=checkbox]', function() {
        
        if (this.id === additionalDeclarationsNotRequiredId) {
            var notRequired = $(this).prop('checked');
            if (notRequired) {
                targetTextBox.val('');

                var searchResultsCheckBoxes = $('[id^="dec-"]');
                searchResultsCheckBoxes.each(function() {
                    $(this).prop('checked',false);
                });
            }
            return;
        }

        var isChecked = $(this).prop('checked');
        const declarationText = $(this).val();

        if (isChecked) {
            const newText = (targetTextBox.val() + newLine + declarationText).trim();
            targetTextBox.val(newText);

            //uncheck the checkbox for does not requires additional declaration.
            additionalDeclarationsNotRequiredCheckBox.prop('checked',false);
        }
        if (!isChecked) {
            var currentText = targetTextBox.val();
            var updatedText = currentText.replace(declarationText, '').trim();
            targetTextBox.val(updatedText);
        }
    });

    $('#Section6_AdditionalDeclarations').on('input change keyup', function () {
        if (this.value.length) {
            if (additionalDeclarationsNotRequiredCheckBox.prop('checked')) {
                //if was checked, then uncheck it now
                additionalDeclarationsNotRequiredCheckBox.prop('checked',false);
            }
        }
    });
})($);