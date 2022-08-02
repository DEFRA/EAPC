(function ($) {
    const line1 = $('#Section2_GoodsInspectionAddress_Line1');
    const line2 = $('#Section2_GoodsInspectionAddress_Line2');
    const line3 = $('#Section2_GoodsInspectionAddress_Line3');
    const line4 = $('#Section2_GoodsInspectionAddress_Line4');
    const postalCode = $('#Section2_GoodsInspectionAddress_PostalCode');
    const inspectionNotRequiredCheckBox = $('#Section2_InspectionNotRequired');

    const handleAddressFieldTextAdded = function(that) {
        if (that.value.length) {
            if (inspectionNotRequiredCheckBox.prop('checked')) {
                inspectionNotRequiredCheckBox.prop('checked', false);
            }
        }
    }

    $(document).on('change', '[type=checkbox]', function() {
        var isChecked = $(this).prop('checked');

        if (isChecked) {
            line1.val('');
            line2.val('');
            line3.val('');
            line4.val('');
            postalCode.val('');
        }
    });

    line1.on('input change keyup', function () { handleAddressFieldTextAdded(this);});
    line2.on('input change keyup', function () { handleAddressFieldTextAdded(this);});
    line3.on('input change keyup', function () { handleAddressFieldTextAdded(this);});
    line4.on('input change keyup', function () { handleAddressFieldTextAdded(this);});
    postalCode.on('input change keyup', function () { handleAddressFieldTextAdded(this);});

})($);