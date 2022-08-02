$(function () {
    function addTextBoxHandler(e) {
        const asJQuery = $(e.target);
        const definedMaxCount = asJQuery.attr('data-max-count');
        const maxAllowed = definedMaxCount === undefined ? 5 : parseInt(definedMaxCount);
        const textBoxes = asJQuery.siblings('input');
        if (textBoxes.length === 0 || textBoxes.length >= maxAllowed) {
            return;
        }

        const textBoxClone = $(textBoxes[0]).clone();
        textBoxClone.val('');
        textBoxClone.attr('id', textBoxes[0].id + '_' + textBoxes.length);
        textBoxClone.insertAfter(textBoxes.get(-1));

        const count = textBoxes.length + 1;
        if (count >= maxAllowed) {
            asJQuery.attr('disabled', 'disabled');
        }
    }

    $('#botanical-names-add-button').on('click', addTextBoxHandler);
    $('#where-grown-add-button').on('click', addTextBoxHandler);
});

$(function() {
    function addQuantityHandler(e) {
        const maxAllowed = 5;
        const asJQuery = $(e.target);
        const divContainers = asJQuery.siblings('div');
        if (divContainers.length === 0 || divContainers.length >= maxAllowed) {
            return;
        }

        const clone = $(divContainers[0]).clone(true);
        clone.find('select')[0].selectedIndex = 0;
        $(clone.find('select')[0]).attr('id', '');
        const inputs = clone.find('input');
        for (let i = 0; i < inputs.length; i++) {
            $(inputs[i]).val('');
            $(inputs[i]).attr('id', '');
        }
        $(clone.find('input[type=text]')[0]).attr('disabled', 'disabled'); // disable the 'other' quantity unit text box

        clone.insertAfter(divContainers.get(-1));

        const count = divContainers.length + 1;
        if (count >= maxAllowed) {
            asJQuery.attr('disabled', 'disabled');
        }

        rebindQuantityIndexes();
    }

    function rebindQuantityIndexes() {
        const containerDiv = $('div[data-purpose="quantity-container"]');
        const amountInputs = containerDiv.find('input[type=number]');
        const unitInputs = containerDiv.find('select');
        const otherTextInputs = containerDiv.find('input[type="text"]');

        for (let i = 0; i < amountInputs.length; i++) {
            $(amountInputs[i]).attr('name', 'Section4.Quantity[' + i + '].Amount');
        }
        for (let i = 0; i < unitInputs.length; i++) {
            $(unitInputs[i]).attr('name', 'Section4.Quantity[' + i + '].Unit');
        }
        for (let i = 0; i < otherTextInputs.length; i++) {
            $(otherTextInputs[i]).attr('name', 'Section4.Quantity[' + i + '].OtherText');
        }
    }

    $('[data-purpose="quantity-unit-selection"]').on('change', function(e) {
        const asJQuery = $(e.target);
        const siblingTextInput = asJQuery.siblings('input[type=text]');
        if (siblingTextInput.length === 0) {
            return;
        }

        $(siblingTextInput[0]).val('');

        if (e.target.value === 'Other') {
            $(siblingTextInput).removeAttr('disabled');
        } else {
            $(siblingTextInput).attr('disabled', 'disabled');
        }
    });
    
    $('#quantity-add-button').on('click', addQuantityHandler);
});

$(function() {
    $('#Section4_MeansOfConveyance').on('change', function(e) {
        const asJQuery = $(e.target);
        const siblingTextInput = asJQuery.siblings('input[type=text]');
        if (siblingTextInput.length === 0) {
            return;
        }

        $(siblingTextInput[0]).val('');

        if (e.target.value === '3') {
            $(siblingTextInput).removeAttr('disabled');
        } else {
            $(siblingTextInput).attr('disabled', 'disabled');
        }
    });
});